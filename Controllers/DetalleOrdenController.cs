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
    public class DetalleOrdenController : Controller
    {
        private readonly CmmsContext _context;

        public DetalleOrdenController(CmmsContext context)
        {
            _context = context;
        }

        // =====================================================
        // 🔵 INDEX CON FILTRO POR ESTADO (INCLUYE CANCELADA)
        // =====================================================
        public async Task<IActionResult> Index(string estado)
        {
            var query = _context.DetalleOrdens
                .Include(d => d.IdAsignacionNavigation)
                .Include(d => d.IdOrdenNavigation)
                .Include(d => d.IdSolicitudNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(d => d.Estado == estado);
            }

            ViewBag.EstadoActual = estado;

            return View(await query.ToListAsync());
        }

        // =====================================================
        // 🔵 DETAILS
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var detalleOrden = await _context.DetalleOrdens
                .Include(d => d.IdAsignacionNavigation)
                .Include(d => d.IdOrdenNavigation)
                .Include(d => d.IdSolicitudNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalle == id);

            if (detalleOrden == null)
                return NotFound();

            return View(detalleOrden);
        }

        // =====================================================
        // 🔵 CREATE
        // =====================================================
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdDetalle,IdOrden,FechaInicio,FechaFin,Descripcion,Estado,CostoTotal,IdAsignacion,IdSolicitud")]
            DetalleOrden detalleOrden)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalleOrden);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarCombos(detalleOrden);
            return View(detalleOrden);
        }

        // =====================================================
        // 🔵 EDIT
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var detalleOrden = await _context.DetalleOrdens.FindAsync(id);

            if (detalleOrden == null)
                return NotFound();

            CargarCombos(detalleOrden);
            return View(detalleOrden);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdDetalle,IdOrden,FechaInicio,FechaFin,Descripcion,Estado,CostoTotal,IdAsignacion,IdSolicitud")]
            DetalleOrden detalleOrden)
        {
            if (id != detalleOrden.IdDetalle)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalleOrden);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetalleOrdenExists(detalleOrden.IdDetalle))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(detalleOrden);
            return View(detalleOrden);
        }

        // =====================================================
        // 🔵 DELETE (SOFT DELETE -> CANCELADA)
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var detalleOrden = await _context.DetalleOrdens
                .Include(d => d.IdAsignacionNavigation)
                .Include(d => d.IdOrdenNavigation)
                .Include(d => d.IdSolicitudNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalle == id);

            if (detalleOrden == null)
                return NotFound();

            return View(detalleOrden);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalleOrden = await _context.DetalleOrdens
                .FirstOrDefaultAsync(d => d.IdDetalle == id);

            if (detalleOrden == null)
                return NotFound();

            // 🔥 CAMBIO DE ESTADO (NO DELETE REAL)
            detalleOrden.Estado = "Cancelada";

            _context.Entry(detalleOrden).Property(x => x.Estado).IsModified = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // 🔧 COMBOS REUTILIZABLES
        // =====================================================
        private void CargarCombos(DetalleOrden? detalleOrden = null)
        {
            ViewData["IdAsignacion"] = new SelectList(
                _context.Asignacions,
                "IdAsignacion",
                "IdAsignacion",
                detalleOrden?.IdAsignacion
            );

            ViewData["IdOrden"] = new SelectList(
                _context.OrdenTrabajos,
                "IdOrden",
                "IdOrden",
                detalleOrden?.IdOrden
            );

            ViewData["IdSolicitud"] = new SelectList(
                _context.SolicitudRepuestos,
                "IdSolicitud",
                "IdSolicitud",
                detalleOrden?.IdSolicitud
            );
        }

        private bool DetalleOrdenExists(int id)
        {
            return _context.DetalleOrdens.Any(e => e.IdDetalle == id);
        }
    }
}