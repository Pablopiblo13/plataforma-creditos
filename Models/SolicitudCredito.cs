using System;
using System.ComponentModel.DataAnnotations;

public enum EstadoSolicitud { Pendiente, Aprobado, Rechazado }

public class SolicitudCredito
{
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "MontoSolicitado debe ser mayor a 0")]
    public decimal MontoSolicitado { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public EstadoSolicitud Estado { get; set; }

    public string MotivoRechazo { get; set; }

    public Cliente Cliente { get; set; }
}
