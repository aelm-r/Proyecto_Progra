==== PROYECTO DE PROGRA IV ====

## Cómo levantar el sistema el día de la feria

### Distribución de las 4 laptops (misma red)

| Laptop | Qué corre | Qué abre en el navegador |
| --- | --- | --- |
| Servidor | Base de datos local (respaldo), backend y frontend | — |
| TV | Nada | `http://<IP del servidor>:4200/tv` |
| Juez | Nada | `http://<IP del servidor>:4200/juez` |
| Administración | Nada | `http://<IP del servidor>:4200/login` |

- Al servidor conviene dejarle una **IP fija** en el router (ej. `192.168.1.50`).
- En el **Firewall de Windows** del servidor hay que permitir los puertos **4200** (frontend) y **5256** (backend).

### 1. Base de datos

**Con internet (Supabase):** es la conexión activa en `Backend/TorneoGallito/appsettings.json`.

**Sin internet (PostgreSQL local en el servidor):**
1. Antes del evento, con internet, copiar Supabase a la base local:
   ```
   createdb -U postgres torneo_gallito
   pg_dump "host=db.knpjxfdsykdckistijng.supabase.co port=5432 dbname=postgres user=postgres sslmode=require" --schema=public --no-owner --no-privileges -f torneo_gallito.sql
   psql -U postgres -d torneo_gallito -f torneo_gallito.sql
   ```
2. En `appsettings.json`, comentar la línea de Supabase y quitar el `//` de la línea de `localhost`
   (ajustar `Password` a la contraseña del PostgreSQL local).

Si la base es nueva: ejecutar `Base de Datos/RobotCompetitionDB.sql` y luego `Base de Datos/correcciones_feria.sql`.

Antes del sorteo deben estar registrados los 8 equipos, cada uno con su robot en estado `activo`.

### 2. Backend (laptop servidor)
```
cd Backend/TorneoGallito
dotnet run --launch-profile http
```
Queda en `http://<IP del servidor>:5256` (Swagger en `/swagger`).

### 3. Frontend (laptop servidor)
Requiere Node.js 22.22.3 o superior (lo exige Angular 22).
```
cd Frontend
npm install
npm run feria
```

El frontend busca el backend en la misma IP con la que se abrió la página. Si el backend corre en otra laptop,
en `Frontend/src/app/core/config.ts` se comenta la primera línea y se pone la IP en la segunda.

### 4. Orden del evento
1. Admin: **Realizar sorteo** → la TV muestra la animación del sorteo.
2. Juez: iniciar combate → iniciar asalto → registrar ganador del asalto → finalizar combate.
3. Al terminar los 12 combates de grupos, Admin: **Generar semifinales**.
4. En eliminatoria, si después de 3 asaltos hay empate, el juez inicia el asalto de muerte súbita (60 s);
   si nadie revienta el globo, se cierra con «Decisión del juez».
