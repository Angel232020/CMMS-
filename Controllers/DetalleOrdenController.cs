using System;
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
        // 🔧 NORMALIZADOR DE ESTADOS
        // =====================================================
        private string NormalizarEstado(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                return "PENDIENTE";

            estado = estado.Trim().ToUpper();

            return estado switch
            {
                "PENDIENTE" => "PENDIENTE",
                "EN PROCESO" => "EN PROCESO",
                "ENPROCESO" => "EN PROCESO",
                "FINALIZADA" => "FINALIZADA",
                "FINALIZADO" => "FINALIZADA",
                "CANCELADA" => "CANCELADA",
                _ => "PENDIENTE"
            };
        }

        // =====================================================
        // INDEX
        // =====================================================
        public async Task<IActionResult> Index(string estado)
        {
            var query = _context.DetalleOrdens
                .AsNoTracking()
                .Include(d => d.IdAsignacionNavigation)
                .Include(d => d.IdOrdenNavigation)
                .Include(d => d.IdSolicitudNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(d => d.Estado == estado);

            return View(await query.ToListAsync());
        }

        // =====================================================
        // DETAILS
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var detalle = await _context.DetalleOrdens
                .AsNoTracking()
                .Include(d => d.IdAsignacionNavigation)
                .Include(d => d.IdOrdenNavigation)
                .Include(d => d.IdSolicitudNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalle == id);

            if (detalle == null)
                return NotFound();

            return View(detalle);
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
        public async Task<IActionResult> Create(DetalleOrden detalleOrden)
        {
            if (ModelState.IsValid)
            {
                detalleOrden.Estado = NormalizarEstado(detalleOrden.Estado);

                _context.Add(detalleOrden);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(detalleOrden);
            return View(detalleOrden);
        }

        // =====================================================
        // EDIT (GET)
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var detalle = await _context.DetalleOrdens.FindAsync(id);

            if (detalle == null)
                return NotFound();

            // 🔴 BLOQUEO REAL
            if (NormalizarEstado(detalle.Estado) == "FINALIZADA")
                return RedirectToAction(nameof(Index));

            CargarCombos(detalle);
            return View(detalle);
        }

        // =====================================================
        // EDIT (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetalleOrden detalleOrden)
        {
            if (id != detalleOrden.IdDetalle)
                return NotFound();

            var db = await _context.DetalleOrdens
                .FirstOrDefaultAsync(x => x.IdDetalle == id);

            if (db == null)
                return NotFound();

            // 🔴 BLOQUEO FINALIZADA
            if (NormalizarEstado(db.Estado) == "FINALIZADA")
                return BadRequest("No se puede editar una orden finalizada");

            if (ModelState.IsValid)
            {
                db.Descripcion = detalleOrden.Descripcion;
                db.CostoTotal = detalleOrden.CostoTotal;
                db.IdAsignacion = detalleOrden.IdAsignacion;
                db.IdOrden = detalleOrden.IdOrden;
                db.IdSolicitud = detalleOrden.IdSolicitud;

                db.Estado = NormalizarEstado(detalleOrden.Estado);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(detalleOrden);
        }

        // =====================================================
        // DELETE (SOFT DELETE)
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var detalle = await _context.DetalleOrdens
                .Include(d => d.IdAsignacionNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalle == id);

            if (detalle == null)
                return NotFound();

            return View(detalle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalle = await _context.DetalleOrdens.FindAsync(id);

            if (detalle == null)
                return NotFound();

            if (NormalizarEstado(detalle.Estado) == "FINALIZADA")
                return BadRequest("No se puede cancelar una orden finalizada");

            detalle.Estado = "CANCELADA";

            _context.Update(detalle);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // INICIAR
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iniciar(int id)
        {
            var detalle = await _context.DetalleOrdens.FindAsync(id);

            if (detalle == null)
                return NotFound();

            if (NormalizarEstado(detalle.Estado) != "PENDIENTE")
                return BadRequest("Solo se puede iniciar una orden pendiente");

            detalle.Estado = "EN PROCESO";
            detalle.FechaInicio = DateTime.Now;

            _context.Update(detalle);
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
            var detalle = await _context.DetalleOrdens.FindAsync(id);

            if (detalle == null)
                return NotFound();

            if (NormalizarEstado(detalle.Estado) != "EN PROCESO")
                return BadRequest("Solo se puede finalizar una orden en proceso");

            detalle.Estado = "FINALIZADA";
            detalle.FechaFin = DateTime.Now;

            _context.Update(detalle);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // COMBOS
        // =====================================================
        private void CargarCombos(DetalleOrden? detalleOrden = null)
        {
            ViewData["IdAsignacion"] = new SelectList(_context.Asignacions, "IdAsignacion", "IdAsignacion", detalleOrden?.IdAsignacion);
            ViewData["IdOrden"] = new SelectList(_context.OrdenTrabajos, "IdOrden", "IdOrden", detalleOrden?.IdOrden);
            ViewData["IdSolicitud"] = new SelectList(_context.SolicitudRepuestos, "IdSolicitud", "IdSolicitud", detalleOrden?.IdSolicitud);
        }

        private bool DetalleOrdenExists(int id)
        {
            return _context.DetalleOrdens.Any(e => e.IdDetalle == id);
        }
    }
}