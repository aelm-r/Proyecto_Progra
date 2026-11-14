CREATE OR REPLACE FUNCTION fn_trigger_auditoria_combates()
RETURNS TRIGGER
SET search_path = public
AS $$
BEGIN
    IF OLD.estado IS DISTINCT FROM NEW.estado THEN
        INSERT INTO auditoria_combates (combate_id, campo, valor_anterior, valor_nuevo, usuario_id)
        VALUES (NEW.id, 'estado', OLD.estado, NEW.estado, NEW.actualizado_por);
    END IF;

    IF OLD.ganador_id IS DISTINCT FROM NEW.ganador_id THEN
        INSERT INTO auditoria_combates (combate_id, campo, valor_anterior, valor_nuevo, usuario_id)
        VALUES (NEW.id, 'ganador_id', OLD.ganador_id::TEXT, NEW.ganador_id::TEXT, NEW.actualizado_por);
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

ALTER TABLE asaltos DROP CONSTRAINT IF EXISTS asaltos_numero_check;
ALTER TABLE asaltos ADD CONSTRAINT asaltos_numero_check CHECK (numero BETWEEN 1 AND 4);

CREATE OR REPLACE FUNCTION fn_tabla_posiciones(p_grupo_id INT)
RETURNS TABLE (
    equipo_id INT,
    nombre_equipo VARCHAR,
    puntos INT,
    asaltos_ganados INT,
    asaltos_perdidos INT,
    diferencia_asaltos INT
) AS $$
#variable_conflict use_column
BEGIN
    RETURN QUERY
    WITH base AS (
        SELECT
            e.id AS eq_id,
            e.nombre AS eq_nombre,
            ge.posicion_sorteo AS pos,
            COALESCE((
                SELECT SUM(CASE
                               WHEN c.ganador_id = e.id THEN 3
                               WHEN c.tipo_resultado = 'empate' THEN 1
                               ELSE 0
                           END)
                FROM combates c
                WHERE c.grupo_id = p_grupo_id
                  AND c.estado = 'finalizado'
                  AND (c.equipo_a_id = e.id OR c.equipo_b_id = e.id)
            ), 0)::INT AS pts,
            (SELECT COUNT(*) FROM asaltos a JOIN combates c ON c.id = a.combate_id
              WHERE c.grupo_id = p_grupo_id AND a.ganador_id = e.id)::INT AS ganados,
            (SELECT COUNT(*) FROM asaltos a JOIN combates c ON c.id = a.combate_id
              WHERE c.grupo_id = p_grupo_id
                AND (c.equipo_a_id = e.id OR c.equipo_b_id = e.id)
                AND a.ganador_id IS NOT NULL AND a.ganador_id <> e.id)::INT AS perdidos,
            (SELECT COUNT(*) FROM amonestaciones am JOIN combates c ON c.id = am.combate_id
              WHERE c.grupo_id = p_grupo_id AND am.equipo_id = e.id)::INT AS faltas
        FROM grupo_equipos ge
        JOIN equipos e ON e.id = ge.equipo_id
        WHERE ge.grupo_id = p_grupo_id
    )
    SELECT b.eq_id, b.eq_nombre, b.pts, b.ganados, b.perdidos, (b.ganados - b.perdidos)::INT
    FROM base b
    ORDER BY b.pts DESC,
             (b.ganados - b.perdidos) DESC,
             b.ganados DESC,
             fn_tiempo_promedio_reventon(b.eq_id) = 0,
             fn_tiempo_promedio_reventon(b.eq_id) ASC,
             b.faltas ASC,
             b.pos ASC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION fn_bracket()
RETURNS TABLE (
    combate_id INT,
    numero_orden INT,
    fase VARCHAR,
    llave VARCHAR,
    equipo_a VARCHAR,
    equipo_b VARCHAR,
    ganador VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        c.id,
        c.numero_orden,
        c.fase,
        c.llave,
        COALESCE(ea.nombre, 'Por definir'),
        COALESCE(eb.nombre, 'Por definir'),
        COALESCE(eg.nombre, 'Pendiente')
    FROM combates c
    LEFT JOIN equipos ea ON c.equipo_a_id = ea.id
    LEFT JOIN equipos eb ON c.equipo_b_id = eb.id
    LEFT JOIN equipos eg ON c.ganador_id = eg.id
    WHERE c.fase IN ('semifinal', 'tercer_lugar', 'final')
    ORDER BY c.numero_orden ASC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION fn_fase_evento()
RETURNS VARCHAR AS $$
DECLARE
    v_total_sorteo INT;
    v_combates_pendientes_grupos INT;
    v_combates_pendientes_elim INT;
BEGIN
    SELECT COUNT(*) INTO v_total_sorteo FROM grupo_equipos;
    IF v_total_sorteo = 0 THEN
        RETURN 'preparacion';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM combates WHERE estado <> 'programado' OR fecha_inicio IS NOT NULL) THEN
        RETURN 'sorteo_realizado';
    END IF;

    SELECT COUNT(*) INTO v_combates_pendientes_grupos
    FROM combates WHERE fase = 'grupos' AND estado NOT IN ('finalizado', 'cancelado');

    IF v_combates_pendientes_grupos > 0 THEN
        RETURN 'grupos';
    END IF;

    SELECT COUNT(*) INTO v_combates_pendientes_elim
    FROM combates WHERE fase IN ('semifinal', 'tercer_lugar', 'final') AND estado NOT IN ('finalizado', 'cancelado');

    IF v_combates_pendientes_elim > 0 THEN
        RETURN 'eliminatoria';
    END IF;

    RETURN 'finalizado';
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION fn_historial_equipo(p_equipo_id INT)
RETURNS TABLE (
    combate_id INT,
    fase VARCHAR,
    rival VARCHAR,
    resultado_combate VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT h.id, h.fase, h.rival, h.resultado
    FROM (
        SELECT c.id, c.fase, eb.nombre AS rival,
               (CASE WHEN c.ganador_id = p_equipo_id THEN 'Victoria'
                     WHEN c.estado <> 'finalizado' THEN 'Pendiente'
                     WHEN c.ganador_id IS NULL THEN 'Empate'
                     ELSE 'Derrota' END)::VARCHAR AS resultado
        FROM combates c
        JOIN equipos eb ON c.equipo_b_id = eb.id
        WHERE c.equipo_a_id = p_equipo_id

        UNION

        SELECT c.id, c.fase, ea.nombre AS rival,
               (CASE WHEN c.ganador_id = p_equipo_id THEN 'Victoria'
                     WHEN c.estado <> 'finalizado' THEN 'Pendiente'
                     WHEN c.ganador_id IS NULL THEN 'Empate'
                     ELSE 'Derrota' END)::VARCHAR AS resultado
        FROM combates c
        JOIN equipos ea ON c.equipo_a_id = ea.id
        WHERE c.equipo_b_id = p_equipo_id
    ) h
    ORDER BY h.id ASC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE sp_iniciar_asalto(p_combate_id INT)
LANGUAGE plpgsql AS $$
DECLARE
    v_estado VARCHAR;
    v_fase VARCHAR;
    v_eq_a INT;
    v_eq_b INT;
    v_score_a INT;
    v_score_b INT;
    v_jugados INT;
    v_muerte_subita BOOLEAN := FALSE;
BEGIN
    SELECT estado, fase, equipo_a_id, equipo_b_id
    INTO v_estado, v_fase, v_eq_a, v_eq_b
    FROM combates WHERE id = p_combate_id;

    IF v_estado IS NULL THEN
        RAISE EXCEPTION 'El combate no existe.';
    END IF;

    IF v_estado NOT IN ('en_curso', 'en_pausa') THEN
        RAISE EXCEPTION 'El combate no está en curso o en pausa.';
    END IF;

    IF EXISTS (SELECT 1 FROM asaltos WHERE combate_id = p_combate_id AND fecha_fin IS NULL) THEN
        RAISE EXCEPTION 'Ya existe un asalto abierto en este combate.';
    END IF;

    SELECT COUNT(*) INTO v_score_a FROM asaltos WHERE combate_id = p_combate_id AND ganador_id = v_eq_a;
    SELECT COUNT(*) INTO v_score_b FROM asaltos WHERE combate_id = p_combate_id AND ganador_id = v_eq_b;
    SELECT COALESCE(MAX(numero), 0) INTO v_jugados FROM asaltos WHERE combate_id = p_combate_id;

    IF v_score_a >= 2 OR v_score_b >= 2 THEN
        RAISE EXCEPTION 'El combate ya está definido: falta finalizarlo.';
    END IF;

    IF v_jugados >= 3 THEN
        IF v_fase = 'grupos' THEN
            RAISE EXCEPTION 'Ya se jugaron los 3 asaltos: falta finalizar el combate.';
        END IF;
        IF v_score_a <> v_score_b THEN
            RAISE EXCEPTION 'El combate ya está definido: falta finalizarlo.';
        END IF;
        IF v_jugados >= 4 THEN
            RAISE EXCEPTION 'La muerte súbita ya se jugó: falta finalizar el combate.';
        END IF;
        v_muerte_subita := TRUE;
    END IF;

    INSERT INTO asaltos (combate_id, numero, es_muerte_subita, fecha_inicio)
    VALUES (p_combate_id, v_jugados + 1, v_muerte_subita, CURRENT_TIMESTAMP);

    UPDATE combates SET estado = 'en_curso' WHERE id = p_combate_id;
END;
$$;

CREATE OR REPLACE PROCEDURE sp_finalizar_combate(p_combate_id INT)
LANGUAGE plpgsql AS $$
DECLARE
    v_fase VARCHAR;
    v_llave VARCHAR;
    v_eq_a INT;
    v_eq_b INT;
    v_ganador INT;
    v_perdedor INT;
    v_score_a INT;
    v_score_b INT;
    v_tipo VARCHAR;
BEGIN
    SELECT fase, llave, equipo_a_id, equipo_b_id
    INTO v_fase, v_llave, v_eq_a, v_eq_b
    FROM combates WHERE id = p_combate_id;

    IF v_eq_a IS NULL OR v_eq_b IS NULL THEN
        RAISE EXCEPTION 'El combate no tiene ambos equipos definidos.';
    END IF;

    SELECT COUNT(*) INTO v_score_a FROM asaltos WHERE combate_id = p_combate_id AND ganador_id = v_eq_a;
    SELECT COUNT(*) INTO v_score_b FROM asaltos WHERE combate_id = p_combate_id AND ganador_id = v_eq_b;

    IF v_score_a > v_score_b THEN
        v_ganador := v_eq_a;
        v_perdedor := v_eq_b;
    ELSIF v_score_b > v_score_a THEN
        v_ganador := v_eq_b;
        v_perdedor := v_eq_a;
    ELSE
        RAISE EXCEPTION 'No puede finalizar un combate en empate técnico sin definir ganador o muerte súbita.';
    END IF;

    SELECT resultado INTO v_tipo
    FROM asaltos
    WHERE combate_id = p_combate_id AND ganador_id = v_ganador
    ORDER BY numero DESC
    LIMIT 1;

    UPDATE combates
    SET estado = 'finalizado',
        ganador_id = v_ganador,
        tipo_resultado = COALESCE(v_tipo, 'decision_juez'),
        fecha_fin = CURRENT_TIMESTAMP
    WHERE id = p_combate_id;

    IF v_fase = 'semifinal' THEN
        IF v_llave = 'SF1' THEN
            UPDATE combates SET equipo_a_id = v_ganador WHERE fase = 'final';
            UPDATE combates SET equipo_a_id = v_perdedor WHERE fase = 'tercer_lugar';
        ELSIF v_llave = 'SF2' THEN
            UPDATE combates SET equipo_b_id = v_ganador WHERE fase = 'final';
            UPDATE combates SET equipo_b_id = v_perdedor WHERE fase = 'tercer_lugar';
        END IF;
    END IF;
END;
$$;

CREATE OR REPLACE PROCEDURE sp_generar_semifinales()
LANGUAGE plpgsql AS $$
DECLARE
    v_pendientes INT;
    v_id_grup_a INT;
    v_id_grup_b INT;
    v_primero_a INT;
    v_segundo_a INT;
    v_primero_b INT;
    v_segundo_b INT;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM combates WHERE fase = 'semifinal') THEN
        RAISE EXCEPTION 'Primero se debe realizar el sorteo.';
    END IF;

    SELECT COUNT(*) INTO v_pendientes
    FROM combates WHERE fase = 'grupos' AND estado NOT IN ('finalizado', 'cancelado');

    IF v_pendientes > 0 THEN
        RAISE EXCEPTION 'Faltan combates de grupos por finalizar. No se pueden generar semifinales.';
    END IF;

    IF EXISTS (SELECT 1 FROM combates WHERE fase = 'semifinal' AND estado <> 'programado') THEN
        RAISE EXCEPTION 'Las semifinales ya empezaron; no se pueden volver a generar.';
    END IF;

    SELECT id INTO v_id_grup_a FROM grupos WHERE nombre = 'A';
    SELECT id INTO v_id_grup_b FROM grupos WHERE nombre = 'B';

    SELECT t.equipo_id INTO v_primero_a FROM fn_tabla_posiciones(v_id_grup_a) t LIMIT 1;
    SELECT t.equipo_id INTO v_segundo_a FROM fn_tabla_posiciones(v_id_grup_a) t OFFSET 1 LIMIT 1;
    SELECT t.equipo_id INTO v_primero_b FROM fn_tabla_posiciones(v_id_grup_b) t LIMIT 1;
    SELECT t.equipo_id INTO v_segundo_b FROM fn_tabla_posiciones(v_id_grup_b) t OFFSET 1 LIMIT 1;

    UPDATE combates SET equipo_a_id = v_primero_a, equipo_b_id = v_segundo_b
    WHERE fase = 'semifinal' AND llave = 'SF1';

    UPDATE combates SET equipo_a_id = v_primero_b, equipo_b_id = v_segundo_a
    WHERE fase = 'semifinal' AND llave = 'SF2';
END;
$$;
