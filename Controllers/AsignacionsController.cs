using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMMS.Models;

namespace CMMS.Controllers
{
    public class AsignacionsController : Controller
    {
        private readonly CmmsContext _context;

        public AsignacionsController(CmmsContext context)
        {
            _context = context;
        }

        // =====================================================
        // INDEX (ADMIN VE TODO / TECNICO SOLO LO SUYO)
        // =====================================================
        public async Task<IActionResult> Index(int? idTecnico, string estado)
        {
            var idRol = User.FindFirst("IdRol")?.Value;
            var idUsuario = User.FindFirst("IdUsuario")?.Value;

            var query = _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .AsQueryable();

            bool esTecnico = idRol == "2";

            // =====================================================
            // 🔥 OBTENER IdTecnico DESDE Usuario
            // =====================================================
            int? tecnicoLogueado = null;

            if (esTecnico && int.TryParse(idUsuario, out int userId))
            {
                tecnicoLogueado = _context.Tecnicos
                    .Where(t => t.id_usuario == userId)
                    .Select(t => t.IdTecnico)
                    .FirstOrDefault();
            }

            // =====================================================
            // 🔧 FILTRO POR ROL
            // =====================================================
            if (esTecnico)
            {
                query = query.Where(a => a.IdTecnico == tecnicoLogueado);
            }

            // =====================================================
            // FILTRO POR TECNICO (ADMIN PUEDE USARLO)
            // =====================================================
            if (!esTecnico && idTecnico.HasValue)
            {
                query = query.Where(a => a.IdTecnico == idTecnico);
            }

            // =====================================================
            // FILTRO POR ESTADO
            // =====================================================
            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(a => a.Estado == estado);
            }

            ViewBag.EstadoActual = estado;
            ViewBag.TecnicoActual = idTecnico;

            ViewData["Tecnicos"] = new SelectList(_context.Tecnicos, "IdTecnico", "Nombres");

            return View(await query.ToListAsync());
        }

        // =====================================================
        // DETAILS
        // =====================================================
        public async Task<IActionResult> Details(int id)
        {
            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(a => a.IdAsignacion == id);

            if (asignacion == null)
                return NotFound();

            if (!PuedeAcceder(asignacion))
                return Forbid();

            return View(asignacion);
        }

        // =====================================================
        // CREATE
        // =====================================================
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asignacion asignacion)
        {
            if (ModelState.IsValid)
            {
                asignacion.FechaAsignacion = DateTime.Now;
                asignacion.Estado = "PENDIENTE";

                _context.Add(asignacion);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(asignacion);
            return View(asignacion);
        }

        // =====================================================
        // INICIAR
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Iniciar(int id)
        {
            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .FirstOrDefaultAsync(a => a.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            if (!PuedeAcceder(asignacion))
                return Forbid();

            asignacion.Estado = "EN PROCESO";

            if (asignacion.IdOrdenNavigation != null)
                asignacion.IdOrdenNavigation.Estado = "EN PROCESO";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // FINALIZAR
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Finalizar(int id)
        {
            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .FirstOrDefaultAsync(a => a.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            if (!PuedeAcceder(asignacion))
                return Forbid();

            asignacion.Estado = "FINALIZADA";

            if (asignacion.IdOrdenNavigation != null)
                asignacion.IdOrdenNavigation.Estado = "FINALIZADA";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // CANCELAR
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Cancelar(int id)
        {
            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .FirstOrDefaultAsync(a => a.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            if (!PuedeAcceder(asignacion))
                return Forbid();

            asignacion.Estado = "CANCELADA";

            if (asignacion.IdOrdenNavigation != null)
                asignacion.IdOrdenNavigation.Estado = "CANCELADA";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // SEGURIDAD CENTRAL
        // =====================================================
        private bool PuedeAcceder(Asignacion asignacion)
        {
            var idRol = User.FindFirst("IdRol")?.Value;
            var idUsuario = User.FindFirst("IdUsuario")?.Value;

            // Admin ve todo
            if (idRol == "1")
                return true;

            // Técnico solo lo suyo
            if (idRol == "2" && int.TryParse(idUsuario, out int userId))
            {
                var tecnicoId = _context.Tecnicos
                    .Where(t => t.id_usuario == userId)
                    .Select(t => t.IdTecnico)
                    .FirstOrDefault();

                return asignacion.IdTecnico == tecnicoId;
            }

            return false;
        }

        // =====================================================
        // COMBOS
        // =====================================================
        private void CargarCombos(Asignacion? asignacion = null)
        {
            ViewData["IdOrden"] = new SelectList(
                _context.OrdenTrabajos.Where(o => o.Estado == "PENDIENTE"),
                "IdOrden",
                "IdOrden",
                asignacion?.IdOrden
            );

            ViewData["IdTecnico"] = new SelectList(
                _context.Tecnicos,
                "IdTecnico",
                "Nombres",
                asignacion?.IdTecnico
            );
        }
    }
}