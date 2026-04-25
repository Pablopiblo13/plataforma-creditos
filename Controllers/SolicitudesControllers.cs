using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PlataformaCreditos.Data;
using Microsoft.EntityFrameworkCore;

namespace PlataformaCreditos.Controllers
{
    [Authorize]
    public class SolicitudesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Vista "Mis solicitudes" con filtros
        public async Task<IActionResult> MisSolicitudes(string estado, decimal? montoMin, decimal? montoMax, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var userId = User.Identity.Name;
            var query = _context.SolicitudesCredito.Include(s => s.Cliente)
                .Where(s => s.Cliente.UsuarioId == userId);

            // Validaciones server-side
            if (montoMin < 0 || montoMax < 0)
                ModelState.AddModelError("", "Montos no pueden ser negativos");
            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio > fechaFin)
                ModelState.AddModelError("", "Rango de fechas inválido");

            if (!ModelState.IsValid)
                return View(await query.ToListAsync());

            // Filtros
            if (!string.IsNullOrEmpty(estado))
                query = query.Where(s => s.Estado.ToString() == estado);
            if (montoMin.HasValue)
                query = query.Where(s => s.MontoSolicitado >= montoMin);
            if (montoMax.HasValue)
                query = query.Where(s => s.MontoSolicitado <= montoMax);
            if (fechaInicio.HasValue)
                query = query.Where(s => s.FechaSolicitud >= fechaInicio);
            if (fechaFin.HasValue)
                query = query.Where(s => s.FechaSolicitud <= fechaFin);

            return View(await query.ToListAsync());
        }

        // Vista detalle
        public async Task<IActionResult> Detalle(int id)
        {
            var solicitud = await _context.SolicitudesCredito.Include(s => s.Cliente)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null) return NotFound();

            return View(solicitud);
        }
    }
}
