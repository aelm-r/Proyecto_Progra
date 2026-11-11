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

        public ManejoErroresMiddleware(RequestDelegate next, ILogger<ManejoErroresMiddleware> logger)
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
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                int codigo = StatusCodes.Status500InternalServerError;
                string mensaje = "Error interno del servidor.";

                if (ex is DatosInvalidosException)
                {
                    codigo = StatusCodes.Status400BadRequest;
                    mensaje = ex.Message;
                }
                else if (ex is UnauthorizedAccessException)
                {
                    codigo = StatusCodes.Status401Unauthorized;
                    mensaje = ex.Message;
                }
                else if (ex is NoEncontradoException)
                {
                    codigo = StatusCodes.Status404NotFound;
                    mensaje = ex.Message;
                }
                else if (ex is ConflictoException)
                {
                    codigo = StatusCodes.Status409Conflict;
                    mensaje = ex.Message;
                }
                else if (ex is PostgresException)
                {
                    PostgresException errorBD = (PostgresException)ex;
                    codigo = CodigoDeErrorBD(errorBD);
                    mensaje = MensajeDeErrorBD(errorBD);
                }
                else if (ex is DbUpdateException && ex.InnerException is PostgresException)
                {
                    PostgresException errorBD = (PostgresException)ex.InnerException;
                    codigo = CodigoDeErrorBD(errorBD);
                    mensaje = MensajeDeErrorBD(errorBD);
                }

                if (codigo == StatusCodes.Status500InternalServerError)
                {
                    _logger.LogError(ex, "Error no controlado");
                }

                context.Response.StatusCode = codigo;
                await context.Response.WriteAsJsonAsync(new MensajeResponseDTO(mensaje));
            }
        }

        private static int CodigoDeErrorBD(PostgresException errorBD)
        {
            if (errorBD.SqlState == PostgresErrorCodes.RaiseException)
            {
                return StatusCodes.Status409Conflict;
            }
            if (errorBD.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return StatusCodes.Status409Conflict;
            }
            if (errorBD.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return StatusCodes.Status409Conflict;
            }
            if (errorBD.SqlState == PostgresErrorCodes.CheckViolation)
            {
                return StatusCodes.Status400BadRequest;
            }
            return StatusCodes.Status500InternalServerError;
        }

        private static string MensajeDeErrorBD(PostgresException errorBD)
        {
            if (errorBD.SqlState == PostgresErrorCodes.RaiseException)
            {
                return errorBD.MessageText;
            }
            if (errorBD.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return "Ya eixste un registro con ese valor.";
            }
            if (errorBD.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return "El registro está relacionado con otros datos.";
            }
            if (errorBD.SqlState == PostgresErrorCodes.CheckViolation)
            {
                return "Un valor no cumple las reglas de la base de dato.";
            }
            return "Error de base de datos.";
        }
    }
}
