using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlataformaCreditos.Models;

namespace PlataformaCreditos.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<SolicitudCredito> SolicitudesCredito => Set<SolicitudCredito>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>()
            .HasCheckConstraint("CK_Cliente_Ingresos", "IngresosMensuales > 0");

        modelBuilder.Entity<SolicitudCredito>()
            .HasCheckConstraint("CK_SolicitudCredito_Monto", "MontoSolicitado > 0");

        // ✅ Solo una solicitud pendiente por cliente
        modelBuilder.Entity<SolicitudCredito>()
            .HasIndex(s => new { s.ClienteId, s.Estado })
            .IsUnique()
            .HasFilter("[Estado] = 0");
    }
}