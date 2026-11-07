namespace TorneoGallito.Models;
// PROVISIONAL: reemplazar por entidad oficial de Catálogos.
public class Equipo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}
