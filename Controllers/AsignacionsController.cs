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

        // ===========================
        // INDEX
        // ===========================
        public async Task<IActionResult> Index(int? idTecnico, string estado)
        {
            var query = _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(a => a.Estado == estado);

            if (idTecnico.HasValue)
                query = query.Where(a => a.IdTecnico == idTecnico);

            ViewBag.EstadoActual = estado;
            ViewBag.TecnicoActual = idTecnico;

            ViewData["Tecnicos"] = new SelectList(_context.Tecnicos, "IdTecnico", "Nombres");

            return View(await query.ToListAsync());
        }

        // ===========================
        // DETAILS  👈 AQUÍ ESTABA EL PROBLEMA
        // ===========================
        public async Task<IActionResult> Details(int id)
        {
            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(a => a.IdAsignacion == id);

            if (asignacion == null)
                return NotFound();

            return View(asignacion);
        }

        // ===========================
        // CREATE GET
        // ===========================
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        // ===========================
        // CREATE POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asignacion asignacion)
        {
            if (ModelState.IsValid)
            {
                asignacion.FechaAsignacion = DateTime.Now;
                asignacion.Estado = "ACTIVA";

                _context.Add(asignacion);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(asignacion);
            return View(asignacion);
        }

        // ===========================
        // INICIAR
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iniciar(int id)
        {
            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .FirstOrDefaultAsync(a => a.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            if (asignacion.Estado == "CANCELADA")
                return BadRequest("Asignación cancelada");

            asignacion.Estado = "EN PROCESO";

            if (asignacion.IdOrdenNavigation != null)
                asignacion.IdOrdenNavigation.Estado = "EN PROCESO";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // FINALIZAR
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .FirstOrDefaultAsync(a => a.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            asignacion.Estado = "FINALIZADA";

            if (asignacion.IdOrdenNavigation != null)
                asignacion.IdOrdenNavigation.Estado = "FINALIZADA";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // EDIT GET
        // ===========================
        public async Task<IActionResult> Edit(int id)
        {
            var asignacion = await _context.Asignacions.FindAsync(id);

            if (asignacion == null) return NotFound();

            if (asignacion.Estado == "FINALIZADA")
                return RedirectToAction(nameof(Index));

            CargarCombos(asignacion);
            return View(asignacion);
        }

        // ===========================
        // EDIT POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Asignacion model)
        {
            var db = await _context.Asignacions.FindAsync(id);

            if (db == null) return NotFound();

            if (db.Estado == "FINALIZADA")
                return BadRequest("No se puede editar");

            if (ModelState.IsValid)
            {
                db.IdTecnico = model.IdTecnico;
                db.IdUsuario = model.IdUsuario;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarCombos(model);
            return View(model);
        }

        // ===========================
        // COMBOS
        // ===========================
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

            ViewData["IdUsuario"] = new SelectList(
                _context.Usuarios,
                "IdUsuario",
                "Nombre",
                asignacion?.IdUsuario
            );
        }
    }
}