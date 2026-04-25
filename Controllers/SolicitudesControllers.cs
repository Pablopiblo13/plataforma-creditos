using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PlataformaCreditos.Data;
using PlataformaCreditos.Models;

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

        // ✅ Vista "Mis Solicitudes" con filtros y validaciones
        public async Task<IActionResult> MisSolicitudes(
            string? estado,
            decimal? montoMin,
            decimal? montoMax,
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.SolicitudesCredito
                .Include(s => s.Cliente)
                .Where(s => s.Cliente.UsuarioId == userId)
                .AsQueryable();

            // ✅ Validaciones server-side
            if (montoMin < 0 || montoMax < 0)
                ModelState.AddModelError("", "Los montos no pueden ser negativos.");

            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio > fechaFin)
                ModelState.AddModelError("", "Rango de fechas inválido.");

            if (!ModelState.IsValid)
                return View(await query.ToListAsync());

            // ✅ Filtros
            if (!string.IsNullOrEmpty(estado)
                && Enum.TryParse<EstadoSolicitud>(estado, out var estadoEnum))
            {
                query = query.Where(s => s.Estado == estadoEnum);
            }

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

        // ✅ Vista Detalle
        public async Task<IActionResult> Detalle(int id)
        {
            var solicitud = await _context.SolicitudesCredito
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null)
                return NotFound();

            return View(solicitud);
        }

        // =====================================================
        // ✅ PREGUNTA 3 – Crear nueva solicitud
        // =====================================================

        // GET
        public IActionResult Crear()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Crear(decimal montoSolicitado)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (cliente == null || !cliente.Activo)
            {
                ModelState.AddModelError("", "Cliente no válido o inactivo.");
                return View();
            }

            var tienePendiente = await _context.SolicitudesCredito
                .AnyAsync(s => s.ClienteId == cliente.Id &&
                               s.Estado == EstadoSolicitud.Pendiente);

            if (tienePendiente)
            {
                ModelState.AddModelError("", "Ya tiene una solicitud pendiente.");
                return View();
            }

            if (montoSolicitado <= 0)
            {
                ModelState.AddModelError("", "Monto inválido.");
                return View();
            }

            if (montoSolicitado > cliente.IngresosMensuales * 10)
            {
                ModelState.AddModelError("", "El monto excede el límite permitido.");
                return View();
            }

            var solicitud = new SolicitudCredito
            {
                ClienteId = cliente.Id,
                MontoSolicitado = montoSolicitado,
                FechaSolicitud = DateTime.Now,
                Estado = EstadoSolicitud.Pendiente
            };

            _context.SolicitudesCredito.Add(solicitud);
            await _context.SaveChangesAsync();

            ViewBag.Mensaje = "Solicitud registrada correctamente.";
            return View();
        }
    }
}