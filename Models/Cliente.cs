using System.ComponentModel.DataAnnotations;

public class Cliente
{
    public int Id { get; set; }

    [Required]
    public string UsuarioId { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "IngresosMensuales debe ser mayor a 0")]
    public decimal IngresosMensuales { get; set; }

    public bool Activo { get; set; }
}
