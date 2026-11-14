
-- 5.1 TABLAS --
-- 1. Roles
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) UNIQUE NOT NULL CHECK (nombre IN ('Admin', 'Juez'))
);

-- 2. Usuarios
CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    nombre_completo VARCHAR(150) NOT NULL,
    rol_id INT NOT NULL REFERENCES roles(id),
    activo BOOLEAN DEFAULT TRUE
);

-- 3. Equipos
CREATE TABLE equipos (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) UNIQUE NOT NULL,
    lema VARCHAR(255),
    color_hex VARCHAR(7) NOT NULL CHECK (color_hex ~ '^#[0-9A-Fa-f]{6}$'),
    logo_url TEXT
);

-- 4. Integrantes
CREATE TABLE integrantes (
    id SERIAL PRIMARY KEY,
    equipo_id INT NOT NULL REFERENCES equipos(id) ON DELETE CASCADE,
    nombre_completo VARCHAR(150) NOT NULL,
    carne VARCHAR(20) UNIQUE NOT NULL
);

-- 5. Robots
CREATE TABLE robots (
    id SERIAL PRIMARY KEY,
    equipo_id INT UNIQUE NOT NULL REFERENCES equipos(id) ON DELETE CASCADE,
    nombre VARCHAR(100) NOT NULL,
    foto_url TEXT,
    estado VARCHAR(20) NOT NULL DEFAULT 'activo' CHECK (estado IN ('activo', 'en_reparacion', 'descalificado'))
);

-- 6. Grupos
CREATE TABLE grupos (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(10) UNIQUE NOT NULL CHECK (nombre IN ('A', 'B'))
);

-- 7. Grupo Equipos (Sorteo)
CREATE TABLE grupo_equipos (
    grupo_id INT NOT NULL REFERENCES grupos(id),
    equipo_id INT NOT NULL REFERENCES equipos(id) ON DELETE CASCADE,
    posicion_sorteo INT NOT NULL CHECK (posicion_sorteo BETWEEN 1 AND 4),
    PRIMARY KEY (grupo_id, equipo_id),
    CONSTRAINT uq_grupo_posicion UNIQUE (grupo_id, posicion_sorteo)
);

-- 8. Combates
CREATE TABLE combates (
    id SERIAL PRIMARY KEY,
    numero_orden INT UNIQUE NOT NULL CHECK (numero_orden BETWEEN 1 AND 16),
    fase VARCHAR(20) NOT NULL CHECK (fase IN ('grupos', 'semifinal', 'tercer_lugar', 'final')),
    llave VARCHAR(20), -- Ej: 'SF1', 'SF2', 'Final'
    grupo_id INT REFERENCES grupos(id),
    equipo_a_id INT REFERENCES equipos(id),
    equipo_b_id INT REFERENCES equipos(id),
    estado VARCHAR(20) NOT NULL DEFAULT 'programado' CHECK (estado IN ('programado', 'en_curso', 'en_pausa', 'finalizado', 'cancelado')),
    ganador_id INT REFERENCES equipos(id),
    tipo_resultado VARCHAR(30) CHECK (tipo_resultado IN ('reventon', 'autogolpe', 'fuera_arena', 'inmovil', 'sancion', 'nulo', 'decision_juez', 'wo', 'empate')),
    fecha_inicio TIMESTAMP,
    fecha_fin TIMESTAMP,
    actualizado_por INT REFERENCES usuarios(id),
    CONSTRAINT chk_equipos_distintos CHECK (equipo_a_id IS NULL OR equipo_b_id IS NULL OR equipo_a_id <> equipo_b_id)
);

-- 9. Asaltos
CREATE TABLE asaltos (
    id SERIAL PRIMARY KEY,
    combate_id INT NOT NULL REFERENCES combates(id) ON DELETE CASCADE,
    numero INT NOT NULL CHECK (numero BETWEEN 1 AND 3),
    es_muerte_subita BOOLEAN DEFAULT FALSE,
    resultado VARCHAR(30) CHECK (resultado IN ('reventon', 'autogolpe', 'fuera_arena', 'inmovil', 'sancion', 'nulo', 'decision_juez', 'wo')),
    ganador_id INT REFERENCES equipos(id),
    fecha_inicio TIMESTAMP,
    fecha_fin TIMESTAMP,
    CONSTRAINT uq_combate_asalto UNIQUE (combate_id, numero)
);

-- 10. Amonestaciones
CREATE TABLE amonestaciones (
    id SERIAL PRIMARY KEY,
    combate_id INT NOT NULL REFERENCES combates(id) ON DELETE CASCADE,
    asalto_id INT REFERENCES asaltos(id) ON DELETE CASCADE,
    equipo_id INT NOT NULL REFERENCES equipos(id),
    motivo TEXT NOT NULL,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 11. Pausas Técnicas
CREATE TABLE pausas_tecnicas (
    id SERIAL PRIMARY KEY,
    combate_id INT NOT NULL REFERENCES combates(id) ON DELETE CASCADE,
    equipo_id INT NOT NULL REFERENCES equipos(id),
    fecha_inicio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_fin TIMESTAMP
);

-- 12. Auditoría de Combates
CREATE TABLE auditoria_combates (
    id SERIAL PRIMARY KEY,
    combate_id INT NOT NULL REFERENCES combates(id),
    campo VARCHAR(50) NOT NULL,
    valor_anterior TEXT,
    valor_nuevo TEXT,
    usuario_id INT REFERENCES usuarios(id),
    fecha_cambio TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 5.2 FUNCIONES (SELECT)
-- 1) fn_tabla_posiciones(grupo) -- 
CREATE OR REPLACE FUNCTION fn_tabla_posiciones(p_grupo_id INT)
RETURNS TABLE (
    equipo_id INT,
    nombre_equipo VARCHAR,
    puntos INT,
    asaltos_ganados INT,
    asaltos_perdidos INT,
    diferencia_asaltos INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        e.id AS equipo_id,
        e.nombre AS nombre_equipo,
        COALESCE(SUM(
            CASE 
                WHEN c.ganador_id = e.id THEN 3
                WHEN c.tipo_resultado = 'empate' AND (c.equipo_a_id = e.id OR c.equipo_b_id = e.id) THEN 1
                ELSE 0
            END
        ), 0)::INT AS puntos,
        -- Contar asaltos ganados
        (SELECT COUNT(*)::INT FROM asaltos a JOIN combates cm ON a.combate_id = cm.id WHERE a.ganador_id = e.id AND cm.grupo_id = p_grupo_id) AS asaltos_ganados,
        -- Contar asaltos perdidos (donde jugó pero ganó el rival)
        (SELECT COUNT(*)::INT FROM asaltos a JOIN combates cm ON a.combate_id = cm.id WHERE (cm.equipo_a_id = e.id OR cm.equipo_b_id = e.id) AND a.ganador_id IS NOT NULL AND a.ganador_id <> e.id AND cm.grupo_id = p_grupo_id) AS asaltos_perdidos,
        -- Diferencia de asaltos
        (
            (SELECT COUNT(*)::INT FROM asaltos a JOIN combates cm ON a.combate_id = cm.id WHERE a.ganador_id = e.id AND cm.grupo_id = p_grupo_id) 
            - 
            (SELECT COUNT(*)::INT FROM asaltos a JOIN combates cm ON a.combate_id = cm.id WHERE (cm.equipo_a_id = e.id OR cm.equipo_b_id = e.id) AND a.ganador_id IS NOT NULL AND a.ganador_id <> e.id AND cm.grupo_id = p_grupo_id)
        )::INT AS diferencia_asaltos
    FROM grupo_equipos ge
    JOIN equipos e ON ge.equipo_id = e.id
    LEFT JOIN combates c ON (c.equipo_a_id = e.id OR c.equipo_b_id = e.id) AND c.grupo_id = p_grupo_id AND c.estado = 'finalizado'
    WHERE ge.grupo_id = p_grupo_id
    GROUP BY e.id, e.nombre, ge.posicion_sorteo
    ORDER BY puntos DESC, diferencia_asaltos DESC, asaltos_ganados DESC, ge.posicion_sorteo ASC;
END;
$$ LANGUAGE plpgsql;

-- 2) fn_siguiente_combate() --
CREATE OR REPLACE FUNCTION fn_siguiente_combate()
RETURNS TABLE (
    combate_id INT,
    numero_orden INT,
    fase VARCHAR,
    equipo_a VARCHAR,
    equipo_b VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c.id,
        c.numero_orden,
        c.fase,
        ea.nombre AS equipo_a,
        eb.nombre AS equipo_b
    FROM combates c
    LEFT JOIN equipos ea ON c.equipo_a_id = ea.id
    LEFT JOIN equipos eb ON c.equipo_b_id = eb.id
    WHERE c.estado = 'programado'
      AND c.equipo_a_id IS NOT NULL 
      AND c.equipo_b_id IS NOT NULL
    ORDER BY c.numero_orden ASC
    LIMIT 1;
END;
$$ LANGUAGE plpgsql;

-- 3)fn_combate_en_curso() --
CREATE OR REPLACE FUNCTION fn_combate_en_curso()
RETURNS TABLE (
    combate_id INT,
    numero_orden INT,
    estado VARCHAR,
    equipo_a VARCHAR,
    equipo_b VARCHAR,
    asalto_actual INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c.id,
        c.numero_orden,
        c.estado,
        ea.nombre,
        eb.nombre,
        (SELECT MAX(a.numero) FROM asaltos a WHERE a.combate_id = c.id)
    FROM combates c
    JOIN equipos ea ON c.equipo_a_id = ea.id
    JOIN equipos eb ON c.equipo_b_id = eb.id
    WHERE c.estado IN ('en_curso', 'en_pausa')
    LIMIT 1;
END;
$$ LANGUAGE plpgsql;

-- 4) fn_bracket() --
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
        COALESCE(ea.name, 'Por definir'),
        COALESCE(eb.name, 'Por definir'),
        COALESCE(eg.name, 'Pendiente')
    FROM combates c
    LEFT JOIN equipos ea ON c.equipo_a_id = ea.id
    LEFT JOIN equipos eb ON c.equipo_b_id = eb.id
    LEFT JOIN equipos eg ON c.ganador_id = eg.id
    WHERE c.fase IN ('semifinal', 'tercer_lugar', 'final')
    ORDER BY c.numero_orden ASC;
END;
$$ LANGUAGE plpgsql;

-- 5) fn_fase_evento() --
CREATE OR REPLACE FUNCTION fn_fase_evento()
RETURNS VARCHAR AS $$
DECLARE
    v_total_sorteo INT;
    v_combates_pendientes_grupos INT;
    v_combates_pendientes_elim INT;
BEGIN
    -- Verificar si se hizo el sorteo
    SELECT COUNT(*) INTO v_total_sorteo FROM grupo_equipos;
    IF v_total_sorteo = 0 THEN
        RETURN 'preparacion';
    END IF;

    -- Verificar combates de grupos pendientes
    SELECT COUNT(*) INTO v_combates_pendientes_grupos 
    FROM combates WHERE fase = 'grupos' AND estado <> 'finalizado';

    IF v_combates_pendientes_grupos > 0 THEN
        RETURN 'grupos';
    END IF;

    -- Verificar combates eliminatorios pendientes
    SELECT COUNT(*) INTO v_combates_pendientes_elim 
    FROM combates WHERE fase IN ('semifinal', 'tercer_lugar', 'final') AND estado <> 'finalizado';

    IF v_combates_pendientes_elim > 0 THEN
        RETURN 'eliminatoria';
    END IF;

    RETURN 'finalizado';
END;
$$ LANGUAGE plpgsql;

-- 6) fn_marcador_combate(combate) --
CREATE OR REPLACE FUNCTION fn_marcador_combate(p_combate_id INT)
RETURNS VARCHAR AS $$
DECLARE
    v_eq_a_id INT;
    v_score_a INT;
    v_score_b INT;
BEGIN
    SELECT equipo_a_id INTO v_eq_a_id FROM combates WHERE id = p_combate_id;

    -- Victorias del equipo A
    SELECT COUNT(*) INTO v_score_a 
    FROM asaltos 
    WHERE combate_id = p_combate_id AND ganador_id = v_eq_a_id;

    -- Victorias del equipo B (cualquier ganador que no sea nulo ni el equipo A)
    SELECT COUNT(*) INTO v_score_b 
    FROM asaltos 
    WHERE combate_id = p_combate_id AND ganador_id IS NOT NULL AND ganador_id <> v_eq_a_id;

    RETURN v_score_a || '-' || v_score_b;
END;
$$ LANGUAGE plpgsql;

-- 7) fn_tiempo_promedio_reventon(equipo) --
CREATE OR REPLACE FUNCTION fn_tiempo_promedio_reventon(p_equipo_id INT)
RETURNS NUMERIC AS $$
DECLARE
    v_promedio NUMERIC;
BEGIN
    SELECT COALESCE(AVG(EXTRACT(EPOCH FROM (fecha_fin - fecha_inicio))), 0)
    INTO v_promedio
    FROM asaltos
    WHERE ganador_id = p_equipo_id 
      AND resultado = 'reventon'
      AND fecha_inicio IS NOT NULL 
      AND fecha_fin IS NOT NULL;

    RETURN ROUND(v_promedio, 2);
END;
$$ LANGUAGE plpgsql;

-- 8) fn_historial_equipo(equipo) --
CREATE OR REPLACE FUNCTION fn_historial_equipo(p_equipo_id INT)
RETURNS TABLE (
    combate_id INT,
    fase VARCHAR,
    rival VARCHAR,
    resultado_combate VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT c.id, c.fase, eb.nombre, 
           CASE WHEN c.ganador_id = p_equipo_id THEN 'Victoria' WHEN c.ganador_id IS NULL THEN 'Pendiente' ELSE 'Derrota' END
    FROM combates c
    JOIN equipos eb ON c.equipo_b_id = eb.id
    WHERE c.equipo_a_id = p_equipo_id
    
    UNION
    
    SELECT c.id, c.fase, ea.nombre, 
           CASE WHEN c.ganador_id = p_equipo_id THEN 'Victoria' WHEN c.ganador_id IS NULL THEN 'Pendiente' ELSE 'Derrota' END
    FROM combates c
    JOIN equipos ea ON c.equipo_a_id = ea.id
    WHERE c.equipo_b_id = p_equipo_id
    
    ORDER BY combate_id ASC;
END;
$$ LANGUAGE plpgsql;

-- 5.3 PROCEDIMIENTOS ALMACENADOS --
-- 1) sp_realizar_sorteo --
CREATE OR REPLACE PROCEDURE sp_realizar_sorteo()
LANGUAGE plpgsql AS $$
DECLARE
    v_conteo_equipos INT;
    v_conteo_sorteo INT;
    v_eq RECORD;
    v_contador INT := 0;
    v_grupo_actual INT;
    v_pos_actual INT;
BEGIN
    -- Validar que no se haya realizado el sorteo antes
    SELECT COUNT(*) INTO v_conteo_sorteo FROM grupo_equipos;
    IF v_conteo_sorteo > 0 THEN
        RAISE EXCEPTION 'El sorteo ya fue realizado previamente.';
    END IF;

    -- Validar que existan exactamente 8 equipos con robot activo
    SELECT COUNT(*) INTO v_conteo_equipos 
    FROM equipos e JOIN robots r ON e.id = r.equipo_id 
    WHERE r.estado = 'activo';

    IF v_conteo_equipos <> 8 THEN
        RAISE EXCEPTION 'Se requieren exactamente 8 equipos con robot activo para realizar el sorteo.';
    END IF;

    -- Limpiar grupos por si acaso e insertar grupos base A y B
    INSERT INTO grupos (nombre) VALUES ('A'), ('B') ON CONFLICT (nombre) DO NOTHING;

    -- Asignar equipos aleatoriamente a los grupos
    FOR v_eq IN 
        SELECT e.id FROM equipos e JOIN robots r ON e.id = r.equipo_id WHERE r.estado = 'activo' ORDER BY RANDOM()
    LOOP
        v_contador := v_contador + 1;
        IF v_contador <= 4 THEN
            v_grupo_actual := (SELECT id FROM grupos WHERE nombre = 'A');
            v_pos_actual := v_contador;
        ELSE
            v_grupo_actual := (SELECT id FROM grupos WHERE nombre = 'B');
            v_pos_actual := v_contador - 4;
        END IF;

        INSERT INTO grupo_equipos (grupo_id, equipo_id, posicion_sorteo)
        VALUES (v_grupo_actual, v_eq.id, v_pos_actual);
     END LOOP;

    -- Generar los 12 combates de fase de grupos (orden fijo según reglamento)
    INSERT INTO combates (numero_orden, fase, grupo_id, equipo_a_id, equipo_b_id, estado)
    SELECT 
        t.orden, 'grupos', g.id, ea.equipo_id, eb.equipo_id, 'programado'
    FROM (
        VALUES 
            (1, 'A', 1, 2), (2, 'B', 1, 2), (3, 'A', 3, 4), (4, 'B', 3, 4),
            (5, 'A', 1, 3), (6, 'B', 1, 3), (7, 'A', 2, 4), (8, 'B', 2, 4),
            (9, 'A', 1, 4), (10, 'B', 1, 4), (11, 'A', 2, 3), (12, 'B', 2, 3)
    ) AS t(orden, grupo_nombre, pos_a, pos_b)
    JOIN grupos g ON g.nombre = t.grupo_nombre
    JOIN grupo_equipos ea ON ea.grupo_id = g.id AND ea.posicion_sorteo = t.pos_a
    JOIN grupo_equipos eb ON eb.grupo_id = g.id AND eb.posicion_sorteo = t.pos_b;

    -- Generar los 4 combates eliminatorios vacíos (Semifinales, Tercer lugar, Final)[cite: 3]
    INSERT INTO combates (numero_orden, fase, llave, estado) VALUES 
    (13, 'semifinal', 'SF1', 'programado'),
    (14, 'semifinal', 'SF2', 'programado'),
    (15, 'tercer_lugar', 'TercerPuesto', 'programado'),
    (16, 'final', 'Final', 'programado');

END;
$$;

-- 2) sp_iniciar_asalto --
CREATE OR REPLACE PROCEDURE sp_iniciar_asalto(p_combate_id INT)
LANGUAGE plpgsql AS $$
DECLARE
    v_estado_combate VARCHAR;
    v_asalto_abierto INT;
    v_siguiente_num INT;
BEGIN
    SELECT estado INTO v_estado_combate FROM combates WHERE id = p_combate_id;

    IF v_estado_combate NOT IN ('en_curso', 'en_pausa') THEN
        RAISE EXCEPTION 'El combate no está en curso o en pausa.';
    END IF;

    -- Verificar si ya hay un asalto abierto (sin fecha_fin)
    SELECT id INTO v_asalto_abierto FROM asaltos WHERE combate_id = p_combate_id AND fecha_fin IS NULL;
    IF v_asalto_abierto IS NOT NULL THEN
        RAISE EXCEPTION 'Ya existe un asalto abierto en este combate.';
    END IF;

    -- Determinar el número de asalto
    SELECT COALESCE(MAX(numero), 0) + 1 INTO v_siguiente_num FROM asaltos WHERE combate_id = p_combate_id;

    IF v_siguiente_num > 3 THEN
        RAISE EXCEPTION 'No se pueden jugar más de 3 asaltos por combate.';
    END IF;

    -- Insertar nuevo asalto
    INSERT INTO asaltos (combate_id, numero, fecha_inicio)
    VALUES (p_combate_id, v_siguiente_num, CURRENT_TIMESTAMP);

    -- Asegurar que el combate quede formalmente en curso
    UPDATE combates SET estado = 'en_curso' WHERE id = p_combate_id;
END;
$$;

-- 3) sp_cerrar_asalto --
CREATE OR REPLACE PROCEDURE sp_cerrar_asalto(
    p_asalto_id INT, 
    p_resultado VARCHAR, 
    p_ganador_id INT
)
LANGUAGE plpgsql AS $$
DECLARE
    v_combate_id INT;
    v_eq_a INT;
    v_eq_b INT;
BEGIN
    SELECT combate_id INTO v_combate_id FROM asaltos WHERE id = p_asalto_id AND fecha_fin IS NULL;
    
    IF v_combate_id IS NULL THEN
        RAISE EXCEPTION 'El asalto no existe o ya se encuentra cerrado.';
    END IF;

    -- Validar que el ganador pertenezca al combate (siempre que no sea nulo)
    IF p_ganador_id IS NOT NULL THEN
        SELECT equipo_a_id, equipo_b_id INTO v_eq_a, v_eq_b FROM combates WHERE id = v_combate_id;
        IF p_ganador_id <> v_eq_a AND p_ganador_id <> v_eq_b THEN
            RAISE EXCEPTION 'El equipo ganador no forma parte de este combate.';
        END IF;
    END IF;

    -- Actualizar el asalto
    UPDATE asaltos 
    SET fecha_fin = CURRENT_TIMESTAMP,
        resultado = p_resultado,
        ganador_id = p_ganador_id
    WHERE id = p_asalto_id;
END;
$$;

-- 4) sp_finalizar_combate --
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
BEGIN
    SELECT fase, llave, equipo_a_id, equipo_b_id 
    INTO v_fase, v_llave, v_eq_a, v_eq_b 
    FROM combates WHERE id = p_combate_id;

    IF v_eq_a IS NULL OR v_eq_b IS NULL THEN
        RAISE EXCEPTION 'El combate no tiene ambos equipos definidos.';
    END IF;

    -- Calcular victorias por asalto
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

    -- Actualizar el combate actual como finalizado
    UPDATE combates 
    SET estado = 'finalizado',
        ganador_id = v_ganador,
        tipo_resultado = 'decision_juez',
        fecha_fin = CURRENT_TIMESTAMP
    WHERE id = p_combate_id;

    -- Lógica para semifinales (actualizar llaves de Final y Tercer Lugar)[cite: 3]
    IF v_fase = 'semifinal' THEN
        IF v_llave = 'SF1' THEN
            -- Ganador de SF1 va a la Final (equipo A), Perdedor va al Tercer Lugar (equipo A)
            UPDATE combates SET equipo_a_id = v_ganador WHERE fase = 'final';
            UPDATE combates SET equipo_a_id = v_perdedor WHERE fase = 'tercer_lugar';
        ELSIF v_llave = 'SF2' THEN
            -- Ganador de SF2 va a la Final (equipo B), Perdedor va al Tercer Lugar (equipo B)
            UPDATE combates SET equipo_b_id = v_ganador WHERE fase = 'final';
            UPDATE combates SET equipo_b_id = v_perdedor WHERE fase = 'tercer_lugar';
        END IF;
    END IF;
END;
$$;

-- 5) sp_registrar_descalificado -- 
CREATE OR REPLACE PROCEDURE sp_registrar_descalificado(p_combate_id INT, p_equipo_id INT)
LANGUAGE plpgsql AS $$
DECLARE
    v_eq_a INT;
    v_eq_b INT;
    v_rival INT;
    v_estado VARCHAR;
BEGIN
    SELECT estado, equipo_a_id, equipo_b_id INTO v_estado, v_eq_a, v_eq_b 
    FROM combates WHERE id = p_combate_id;

    IF v_estado = 'finalizado' THEN
        RAISE EXCEPTION 'El combate ya está finalizado.';
    END IF;

    IF p_equipo_id = v_eq_a THEN
        v_rival := v_eq_b;
    ELSIF p_equipo_id = v_eq_b THEN
        v_rival := v_eq_a;
    ELSE
        RAISE EXCEPTION 'El equipo no pertenece a este combate.';
    END IF;

    -- Marcar el combate como finalizado por W.O.
    UPDATE combates 
    SET estado = 'finalizado',
        ganador_id = v_rival,
        tipo_resultado = 'wo',
        fecha_fin = CURRENT_TIMESTAMP
    WHERE id = p_combate_id;

    -- Registrar asaltos técnicos de castigo (2 a 0)
    DELETE FROM asaltos WHERE combate_id = p_combate_id;
    
    INSERT INTO asaltos (combate_id, numero, resultado, ganador_id, fecha_inicio, fecha_fin)
    VALUES 
    (p_combate_id, 1, 'wo', v_rival, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    (p_combate_id, 2, 'wo', v_rival, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
END;
$$;

-- 6) sp_generar_semifinales --
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
    -- Validar que no falte ningún combate de grupo
    SELECT COUNT(*) INTO v_pendientes 
    FROM combates WHERE fase = 'grupos' AND estado <> 'finalizado';

    IF v_pendientes > 0 THEN
        RAISE EXCEPTION 'Faltan combates de grupos por finalizar. No se pueden generar semifinales.';
    END IF;

    SELECT id INTO v_id_grup_a FROM grupos WHERE nombre = 'A';
    SELECT id INTO v_id_grup_b FROM grupos WHERE nombre = 'B';

    -- Obtener 1.° y 2.° del Grupo A
    SELECT equipo_id INTO v_primero_a FROM fn_tabla_posiciones(v_id_grup_a) LIMIT 1;
    SELECT equipo_id INTO v_segundo_a FROM fn_tabla_posiciones(v_id_grup_a) OFFSET 1 LIMIT 1;

    -- Obtener 1.° y 2.° del Grupo B
    SELECT equipo_id INTO v_primero_b FROM fn_tabla_posiciones(v_id_grup_b) LIMIT 1;
    SELECT equipo_id INTO v_segundo_b FROM fn_tabla_posiciones(v_id_grup_b) OFFSET 1 LIMIT 1;

    -- Asignar Semifinal 1 (1.° A vs 2.° B)[cite: 1, 2]
    UPDATE combates 
    SET equipo_a_id = v_primero_a, 
        equipo_b_id = v_segundo_b 
    WHERE fase = 'semifinal' AND llave = 'SF1';

    -- Asignar Semifinal 2 (1.° B vs 2.° A)[cite: 1, 2]
    UPDATE combates 
    SET equipo_a_id = v_primero_b, 
        equipo_b_id = v_segundo_a 
    WHERE fase = 'semifinal' AND llave = 'SF2';
END;
$$;

-- 5.4 TRIGGER Y REPORTES --
CREATE OR REPLACE FUNCTION fn_trigger_auditoria_combates()
RETURNS TRIGGER AS $$
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

CREATE TRIGGER trg_auditoria_combates
AFTER UPDATE ON combates
FOR EACH ROW
EXECUTE FUNCTION fn_trigger_auditoria_combates();