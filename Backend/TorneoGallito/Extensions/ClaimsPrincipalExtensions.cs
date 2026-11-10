using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace TorneoGallito.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int ObtenerUsuarioId(this ClaimsPrincipal usuario)
        {
            var valor = usuario.FindFirstValue("id");
            if (!int.TryParse(valor, out var id))
                throw new UnauthorizedAccessException("El token no tiene el Id del usuario.");
            return id;
        }
    }
}
