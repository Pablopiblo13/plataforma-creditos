namespace PlataformaCreditos.Models;

public class SolicitudCredito
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public decimal MontoSolicitado { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public EstadoSolicitud Estado { get; set; }

    public string? MotivoRechazo { get; set; }
}