using Microsoft.EntityFrameworkCore;
using Npgsql;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Exceptions;


namespace TorneoGallito.Middleware
{
    public class ManejoErroresMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejoErroresMiddleware> _logger;

        public ManejoErroresMiddleware(RequestDelegate next, ILoggerFactory<ManejoErroresMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (!context.Response.HasStarted)
            {
                var (status, mensaje) = Traducir(ex);
                if (status == StatusCodes.Status500InternalServerError)
                    _logger.LogError(ex, "Error no controlado");
                context.Response.StatusCode = status;
                await context.Response.WriteAsync(new MensajeResponsableDTO(mensaje));
            }
        }

        private static (int, string) Traducir(Exception ex) => ex switch
        {
            DatosInvalidosExceptions e => (StatusCodes.Status400BadRequest, e.Message),
            UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, e.Message),
            NoEncontradoExceptions e => (StatusCodes.Status404NotFound, e.Message),
            ConflictoExceptions e => (StatusCodes.Status409Conflict, e.Message),
            PostgresException pg => TraducirPostgres(pg),
            DbUpdateException { InnerException: PostgresException pg } => TraducirPostgres(pg),
            NotImplementedException => (StatusCodes.Status501NotImplemented, "Este endpoint todavía no está implementado."),
            _ => (StatusCodes.Status500InternalServerError, "Erron interno en el servidor.")
        };

        private static (int, string) TraducirPostgres(PostgresException pg) => pg.SqlState switch
        {
            PostgresErrorCodes.RaiseException => (StatusCodes.Status409Conflict, pg.MessageText),
            PostgresErrorCodes.UniqueViolation => (StatusCodes.Status409Conflict, "Ya esxiste un registro con ese valor"),
            PostgresErrorCodes.ForeignKeyViolation => (StatusCodes.Status409Conflict, "El registro está relacionado con otros datos"),
            PostgresErrorCodes.CheckViolation => (StatusCodes.Status400BadRequest, "Un valor no cumple las reglas de la base de datos"),
            _ => (StatusCodes.Status500InternalServerError, "Error de base de datos")
        };
    }
}
