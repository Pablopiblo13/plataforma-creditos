using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlataformaCreditos.Data;
using PlataformaCreditos.Models;

namespace PlataformaCreditos.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        if (!context.Clientes.Any())
        {
            var cliente1 = new Cliente
            {
                UsuarioId = "user1",
                IngresosMensuales = 2000,
                Activo = true
            };

            var cliente2 = new Cliente
            {
                UsuarioId = "user2",
                IngresosMensuales = 3000,
                Activo = true
            };

            context.Clientes.AddRange(cliente1, cliente2);

            context.SolicitudesCredito.AddRange(
                new SolicitudCredito
                {
                    Cliente = cliente1,
                    MontoSolicitado = 4000,
                    FechaSolicitud = DateTime.Now,
                    Estado = EstadoSolicitud.Pendiente
                },
                new SolicitudCredito
                {
                    Cliente = cliente2,
                    MontoSolicitado = 5000,
                    FechaSolicitud = DateTime.Now,
                    Estado = EstadoSolicitud.Aprobado
                }
            );

            await context.SaveChangesAsync();
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Analista"))
        {
            await roleManager.CreateAsync(new IdentityRole("Analista"));
        }

        var analista = await userManager.FindByNameAsync("analista@demo.com");
        if (analista == null)
        {
            analista = new IdentityUser
            {
                UserName = "analista@demo.com",
                Email = "analista@demo.com",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(analista, "Analista123!");
            await userManager.AddToRoleAsync(analista, "Analista");
        }
    }
}