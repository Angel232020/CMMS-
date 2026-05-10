using System;
using System.Linq;
using System.Threading.Tasks;
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
        // INDEX
        // =====================================================
        public async Task<IActionResult> Index()
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var idUsuarioClaim = User.FindFirst("IdUsuario");
            var idRolClaim = User.FindFirst("IdRol");

            if (idUsuarioClaim == null || idRolClaim == null)
                return RedirectToAction("Login", "Account");

            int idUsuario = int.Parse(idUsuarioClaim.Value);
            int idRol = int.Parse(idRolClaim.Value);

            IQueryable<Asignacion> query = _context.Asignacions
                .AsNoTracking()
                .Where(a => a.Estado == "ACTIVA")
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation);

            if (idRol == 2)
            {
                var tecnico = await _context.Tecnicos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.id_usuario == idUsuario);

                if (tecnico == null)
                    return Forbid();

                query = query.Where(a => a.IdTecnico == tecnico.IdTecnico);
            }

            return View(await query.ToListAsync());
        }

        // =====================================================
        // DETAILS
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var asignacion = await _context.Asignacions
                .AsNoTracking()
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            return View(asignacion);
        }

        // =====================================================
        // CREATE (GET)
        // =====================================================
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        // =====================================================
        // CREATE (POST) - VALIDACIÓN REAL
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asignacion asignacion)
        {
            var orden = await _context.OrdenTrabajos
                .FirstOrDefaultAsync(o => o.IdOrden == asignacion.IdOrden);

            if (orden == null || orden.Estado != "PENDIENTE")
            {
                ModelState.AddModelError("", "Solo se pueden asignar órdenes PENDIENTES");
                CargarCombos(asignacion);
                return View(asignacion);
            }

            if (ModelState.IsValid)
            {
                asignacion.Estado = "ACTIVA";

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iniciar(int id)
        {
            var asignacion = await _context.Asignacions.FindAsync(id);
            if (asignacion == null) return NotFound();

            var orden = await _context.OrdenTrabajos
                .FirstOrDefaultAsync(o => o.IdOrden == asignacion.IdOrden);

            if (orden == null) return NotFound();

            orden.Estado = "EN PROCESO";

            _context.Update(orden);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // FINALIZAR
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            var asignacion = await _context.Asignacions.FindAsync(id);
            if (asignacion == null) return NotFound();

            var orden = await _context.OrdenTrabajos
                .FirstOrDefaultAsync(o => o.IdOrden == asignacion.IdOrden);

            if (orden == null) return NotFound();

            orden.Estado = "FINALIZADA";

            _context.Update(orden);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // EDIT
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var asignacion = await _context.Asignacions.FindAsync(id);
            if (asignacion == null) return NotFound();

            CargarCombos(asignacion);
            return View(asignacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Asignacion asignacion)
        {
            if (id != asignacion.IdAsignacion)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(asignacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarCombos(asignacion);
            return View(asignacion);
        }

        // =====================================================
        // DELETE (SOFT)
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .FirstOrDefaultAsync(m => m.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            return View(asignacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asignacion = await _context.Asignacions.FindAsync(id);

            if (asignacion != null)
            {
                asignacion.Estado = "CANCELADA";
                _context.Update(asignacion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // COMBOS (SOLO PENDIENTES)
        // =====================================================
        private void CargarCombos(Asignacion? asignacion = null)
        {
            // 🔥 SOLO ORDENES PENDIENTES
            ViewData["IdOrden"] = new SelectList(
                _context.OrdenTrabajos
                    .Where(o => o.Estado == "PENDIENTE")
                    .Select(o => new
                    {
                        o.IdOrden,
                        Texto = "Orden #" + o.IdOrden + " - PENDIENTE"
                    }),
                "IdOrden",
                "Texto",
                asignacion?.IdOrden
            );

            ViewData["IdTecnico"] = new SelectList(
                _context.Tecnicos.Select(t => new
                {
                    t.IdTecnico,
                    Texto = t.Nombres + " " + t.Apellidos
                }),
                "IdTecnico",
                "Texto",
                asignacion?.IdTecnico
            );

            ViewData["IdUsuario"] = new SelectList(
                _context.Usuarios.Select(u => new
                {
                    u.IdUsuario,
                    Texto = u.Nombre
                }),
                "IdUsuario",
                "Texto",
                asignacion?.IdUsuario
            );
        }

        private bool AsignacionExists(int id)
        {
            return _context.Asignacions.Any(e => e.IdAsignacion == id);
        }
    }
}