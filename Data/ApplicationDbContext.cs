using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<SolicitudCredito> Solicitudes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Restricciones
        modelBuilder.Entity<Cliente>()
            .HasCheckConstraint("CK_Cliente_Ingresos", "IngresosMensuales > 0");

        modelBuilder.Entity<SolicitudCredito>()
            .HasCheckConstraint("CK_SolicitudCredito_Monto", "MontoSolicitado > 0");

        // Un cliente solo puede tener una solicitud pendiente
        modelBuilder.Entity<SolicitudCredito>()
            .HasIndex(s => new { s.ClienteId, s.Estado })
            .IsUnique()
            .HasFilter("[Estado] = 0"); // 0 = Pendiente
    }
}
