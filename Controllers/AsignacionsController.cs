using System;
using System.Collections.Generic;
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

        // GET: Asignacions
        public async Task<IActionResult> Index()
        {
            var cmmsContext = _context.Asignacions
                .Where(a => a.Estado == "ACTIVA")
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation);

            return View(await cmmsContext.ToListAsync());
        }

        // GET: Asignacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            return View(asignacion);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["IdOrden"] = new SelectList(
                _context.OrdenTrabajos.Select(o => new
                {
                    o.IdOrden,
                    Texto = "Orden #" + o.IdOrden + " - " + o.Estado
                }),
                "IdOrden",
                "Texto"
            );

            ViewData["IdTecnico"] = new SelectList(
                _context.Tecnicos.Select(t => new
                {
                    t.IdTecnico,
                    Texto = t.Nombres + " " + t.Apellidos
                }),
                "IdTecnico",
                "Texto"
            );

            ViewData["IdUsuario"] = new SelectList(
                _context.Usuarios.Select(u => new
                {
                    u.IdUsuario,
                    Texto = u.Nombre
                }),
                "IdUsuario",
                "Texto"
            );

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAsignacion,IdOrden,IdTecnico,FechaAsignacion,IdUsuario")] Asignacion asignacion)
        {
            if (ModelState.IsValid)
            {
                asignacion.Estado = "ACTIVA";
                _context.Add(asignacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(asignacion);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var asignacion = await _context.Asignacions.FindAsync(id);

            if (asignacion == null) return NotFound();

            ViewData["IdOrden"] = new SelectList(
                _context.OrdenTrabajos.Select(o => new
                {
                    o.IdOrden,
                    Texto = "Orden #" + o.IdOrden + " - " + o.Estado
                }),
                "IdOrden",
                "Texto",
                asignacion.IdOrden
            );

            ViewData["IdTecnico"] = new SelectList(
                _context.Tecnicos.Select(t => new
                {
                    t.IdTecnico,
                    Texto = t.Nombres + " " + t.Apellidos
                }),
                "IdTecnico",
                "Texto",
                asignacion.IdTecnico
            );

            ViewData["IdUsuario"] = new SelectList(
                _context.Usuarios.Select(u => new
                {
                    u.IdUsuario,
                    Texto = u.Nombre
                }),
                "IdUsuario",
                "Texto",
                asignacion.IdUsuario
            );

            return View(asignacion);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Asignacion asignacion)
        {
            if (id != asignacion.IdAsignacion)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asignacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsignacionExists(asignacion.IdAsignacion))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(asignacion);
        }

        // GET: Delete (confirmación)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsignacion == id);

            if (asignacion == null) return NotFound();

            return View(asignacion);
        }

        // POST: CANCELAR (soft delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asignacion = await _context.Asignacions.FindAsync(id);

            if (asignacion != null)
            {
                asignacion.Estado = "CANCELADA";
                _context.Update(asignacion);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool AsignacionExists(int id)
        {
            return _context.Asignacions.Any(e => e.IdAsignacion == id);
        }
    }
}