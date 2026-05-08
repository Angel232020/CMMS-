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
    public class SolicitudRepuestoesController : Controller
    {
        private readonly CmmsContext _context;

        public SolicitudRepuestoesController(CmmsContext context)
        {
            _context = context;
        }

        // GET: SolicitudRepuestoes
        public async Task<IActionResult> Index()
        {
            var cmmsContext = _context.SolicitudRepuestos.Include(s => s.IdAsignacionNavigation).Include(s => s.IdRepuestoNavigation);
            return View(await cmmsContext.ToListAsync());
        }

        // GET: SolicitudRepuestoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudRepuesto = await _context.SolicitudRepuestos
                .Include(s => s.IdAsignacionNavigation)
                .Include(s => s.IdRepuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);
            if (solicitudRepuesto == null)
            {
                return NotFound();
            }

            return View(solicitudRepuesto);
        }

        // GET: SolicitudRepuestoes/Create
        public IActionResult Create()
        {
            ViewData["IdAsignacion"] = new SelectList(_context.Asignacions, "IdAsignacion", "IdAsignacion");
            ViewData["IdRepuesto"] = new SelectList(_context.Repuestos, "IdRepuesto", "IdRepuesto");
            return View();
        }

        // POST: SolicitudRepuestoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSolicitud,IdAsignacion,Fecha,IdRepuesto,Cantidad,Comentarios")] SolicitudRepuesto solicitudRepuesto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solicitudRepuesto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsignacion"] = new SelectList(_context.Asignacions, "IdAsignacion", "IdAsignacion", solicitudRepuesto.IdAsignacion);
            ViewData["IdRepuesto"] = new SelectList(_context.Repuestos, "IdRepuesto", "IdRepuesto", solicitudRepuesto.IdRepuesto);
            return View(solicitudRepuesto);
        }

        // GET: SolicitudRepuestoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudRepuesto = await _context.SolicitudRepuestos.FindAsync(id);
            if (solicitudRepuesto == null)
            {
                return NotFound();
            }
            ViewData["IdAsignacion"] = new SelectList(_context.Asignacions, "IdAsignacion", "IdAsignacion", solicitudRepuesto.IdAsignacion);
            ViewData["IdRepuesto"] = new SelectList(_context.Repuestos, "IdRepuesto", "IdRepuesto", solicitudRepuesto.IdRepuesto);
            return View(solicitudRepuesto);
        }

        // POST: SolicitudRepuestoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSolicitud,IdAsignacion,Fecha,IdRepuesto,Cantidad,Comentarios")] SolicitudRepuesto solicitudRepuesto)
        {
            if (id != solicitudRepuesto.IdSolicitud)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solicitudRepuesto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolicitudRepuestoExists(solicitudRepuesto.IdSolicitud))
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
            ViewData["IdAsignacion"] = new SelectList(_context.Asignacions, "IdAsignacion", "IdAsignacion", solicitudRepuesto.IdAsignacion);
            ViewData["IdRepuesto"] = new SelectList(_context.Repuestos, "IdRepuesto", "IdRepuesto", solicitudRepuesto.IdRepuesto);
            return View(solicitudRepuesto);
        }

        // GET: SolicitudRepuestoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudRepuesto = await _context.SolicitudRepuestos
                .Include(s => s.IdAsignacionNavigation)
                .Include(s => s.IdRepuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);
            if (solicitudRepuesto == null)
            {
                return NotFound();
            }

            return View(solicitudRepuesto);
        }

        // POST: SolicitudRepuestoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solicitudRepuesto = await _context.SolicitudRepuestos.FindAsync(id);
            if (solicitudRepuesto != null)
            {
                _context.SolicitudRepuestos.Remove(solicitudRepuesto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SolicitudRepuestoExists(int id)
        {
            return _context.SolicitudRepuestos.Any(e => e.IdSolicitud == id);
        }
    }
}
