using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        // =========================
        // LISTADO
        // =========================
        public async Task<IActionResult> Index()
        {
            var data = _context.SolicitudRepuestos
                .Include(s => s.IdAsignacionNavigation)
                .Include(s => s.IdRepuestoNavigation);

            return View(await data.ToListAsync());
        }

        // =========================
        // DETALLE
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var solicitud = await _context.SolicitudRepuestos
                .Include(s => s.IdAsignacionNavigation)
                .Include(s => s.IdRepuestoNavigation)
                .FirstOrDefaultAsync(s => s.IdSolicitud == id);

            if (solicitud == null)
                return NotFound();

            return View(solicitud);
        }

        // =========================
        // CREATE (DESDE ORDEN)
        // =========================
        public IActionResult Create(int idAsignacion)
        {
            if (idAsignacion <= 0)
                return NotFound();

            var model = new SolicitudRepuesto
            {
                IdAsignacion = idAsignacion,
                Fecha = DateTime.Now
            };

            ViewBag.Repuestos = _context.Repuestos.ToList();

            return View(model);
        }

        // =========================
        // POST CREATE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolicitudRepuesto solicitud)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Repuestos = _context.Repuestos.ToList();
                return View(solicitud);
            }

            // 🔥 FECHA AUTOMÁTICA
            solicitud.Fecha = DateTime.Now;

            // =========================
            // 🔥 VALIDAR STOCK
            // =========================
            var repuesto = await _context.Repuestos
                .FirstOrDefaultAsync(r => r.IdRepuesto == solicitud.IdRepuesto);

            if (repuesto == null)
            {
                ModelState.AddModelError("", "Repuesto no encontrado");
                ViewBag.Repuestos = _context.Repuestos.ToList();
                return View(solicitud);
            }

            if (repuesto.Stock < solicitud.Cantidad)
            {
                ModelState.AddModelError("", "Stock insuficiente");
                ViewBag.Repuestos = _context.Repuestos.ToList();
                return View(solicitud);
            }

            // =========================
            // 🔥 GUARDAR SOLICITUD
            // =========================
            _context.SolicitudRepuestos.Add(solicitud);

            // 🔥 DESCONTAR STOCK
            repuesto.Stock -= solicitud.Cantidad;

            await _context.SaveChangesAsync();

            // =========================
            // 🔥 REGRESAR AL DETALLE DE ORDEN
            // =========================
            var ordenId = await _context.Asignacions
                .Where(a => a.IdAsignacion == solicitud.IdAsignacion)
                .Select(a => a.IdOrden)
                .FirstOrDefaultAsync();

            return RedirectToAction("Details", "OrdenTrabajoes", new { id = ordenId });
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var solicitud = await _context.SolicitudRepuestos.FindAsync(id);

            if (solicitud == null)
                return NotFound();

            ViewBag.Repuestos = _context.Repuestos.ToList();

            return View(solicitud);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SolicitudRepuesto solicitud)
        {
            if (id != solicitud.IdSolicitud)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Repuestos = _context.Repuestos.ToList();
                return View(solicitud);
            }

            try
            {
                _context.Update(solicitud);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.SolicitudRepuestos.Any(e => e.IdSolicitud == id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var solicitud = await _context.SolicitudRepuestos
                .Include(s => s.IdAsignacionNavigation)
                .Include(s => s.IdRepuestoNavigation)
                .FirstOrDefaultAsync(s => s.IdSolicitud == id);

            if (solicitud == null)
                return NotFound();

            return View(solicitud);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solicitud = await _context.SolicitudRepuestos.FindAsync(id);

            if (solicitud != null)
            {
                // 🔥 DEVOLVER STOCK
                var repuesto = await _context.Repuestos
                    .FirstOrDefaultAsync(r => r.IdRepuesto == solicitud.IdRepuesto);

                if (repuesto != null)
                {
                    repuesto.Stock += solicitud.Cantidad;
                }

                _context.SolicitudRepuestos.Remove(solicitud);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}