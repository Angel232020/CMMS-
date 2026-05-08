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
    public class AsignacionController : Controller
    {
        private readonly CmmsContext _context;

        public AsignacionController(CmmsContext context)
        {
            _context = context;
        }

        // GET: Asignacions1
        public async Task<IActionResult> Index()
        {
            var cmmsContext = _context.Asignacions.Include(a => a.IdOrdenNavigation).Include(a => a.IdTecnicoNavigation).Include(a => a.IdUsuarioNavigation);
            return View(await cmmsContext.ToListAsync());
        }

        // GET: Asignacions1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsignacion == id);
            if (asignacion == null)
            {
                return NotFound();
            }

            return View(asignacion);
        }

        // GET: Asignacions1/Create
        public IActionResult Create()
        {
            ViewData["IdOrden"] = new SelectList(_context.OrdenTrabajos, "IdOrden", "IdOrden");
            ViewData["IdTecnico"] = new SelectList(_context.Tecnicos, "IdTecnico", "IdTecnico");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: Asignacions1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAsignacion,IdOrden,IdTecnico,FechaAsignacion,IdUsuario")] Asignacion asignacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asignacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdOrden"] = new SelectList(_context.OrdenTrabajos, "IdOrden", "IdOrden", asignacion.IdOrden);
            ViewData["IdTecnico"] = new SelectList(_context.Tecnicos, "IdTecnico", "IdTecnico", asignacion.IdTecnico);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", asignacion.IdUsuario);
            return View(asignacion);
        }

        // GET: Asignacions1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignacion = await _context.Asignacions.FindAsync(id);
            if (asignacion == null)
            {
                return NotFound();
            }
            ViewData["IdOrden"] = new SelectList(_context.OrdenTrabajos, "IdOrden", "IdOrden", asignacion.IdOrden);
            ViewData["IdTecnico"] = new SelectList(_context.Tecnicos, "IdTecnico", "IdTecnico", asignacion.IdTecnico);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", asignacion.IdUsuario);
            return View(asignacion);
        }

        // POST: Asignacions1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsignacion,IdOrden,IdTecnico,FechaAsignacion,IdUsuario")] Asignacion asignacion)
        {
            if (id != asignacion.IdAsignacion)
            {
                return NotFound();
            }

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
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdOrden"] = new SelectList(_context.OrdenTrabajos, "IdOrden", "IdOrden", asignacion.IdOrden);
            ViewData["IdTecnico"] = new SelectList(_context.Tecnicos, "IdTecnico", "IdTecnico", asignacion.IdTecnico);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", asignacion.IdUsuario);
            return View(asignacion);
        }

        // GET: Asignacions1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignacion = await _context.Asignacions
                .Include(a => a.IdOrdenNavigation)
                .Include(a => a.IdTecnicoNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsignacion == id);
            if (asignacion == null)
            {
                return NotFound();
            }

            return View(asignacion);
        }

        // POST: Asignacions1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asignacion = await _context.Asignacions.FindAsync(id);
            if (asignacion != null)
            {
                _context.Asignacions.Remove(asignacion);
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
