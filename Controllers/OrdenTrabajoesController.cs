using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMMS.Models;

namespace CMMS.Controllers
{
    public class OrdenTrabajoesController : Controller
    {
        private readonly CmmsContext _context;

        public OrdenTrabajoesController(CmmsContext context)
        {
            _context = context;
        }

        // =====================================================
        // INDEX
        // =====================================================
        public async Task<IActionResult> Index(string estado)
        {
            var ordenes = _context.OrdenTrabajos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.IdMaquinaNavigation)
                .Include(o => o.IdTipoServicioNavigation)
                .Include(o => o.IdUsuarioNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                ordenes = ordenes.Where(o => o.Estado == estado);
            }

            ViewBag.EstadoActual = estado;

            return View(await ordenes.ToListAsync());
        }

        // =====================================================
        // DETAILS
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ordenTrabajo = await _context.OrdenTrabajos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.IdMaquinaNavigation)
                .Include(o => o.IdTipoServicioNavigation)
                .Include(o => o.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdOrden == id);

            if (ordenTrabajo == null)
                return NotFound();

            return View(ordenTrabajo);
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
        // CREATE (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrdenTrabajo ordenTrabajo)
        {
            if (ModelState.IsValid)
            {
                // 🔥 SIEMPRE CONTROLADO POR SISTEMA
                ordenTrabajo.Estado = "PENDIENTE";

                _context.Add(ordenTrabajo);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(ordenTrabajo);
            return View(ordenTrabajo);
        }

        // =====================================================
        // EDIT (GET)
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ordenTrabajo = await _context.OrdenTrabajos.FindAsync(id);

            if (ordenTrabajo == null)
                return NotFound();

            // 🔒 BLOQUEO REAL
            if (ordenTrabajo.Estado?.Trim().ToUpper() == "FINALIZADA")
                return RedirectToAction(nameof(Index));

            CargarCombos(ordenTrabajo);
            return View(ordenTrabajo);
        }

        // =====================================================
        // EDIT (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrdenTrabajo ordenTrabajo)
        {
            if (id != ordenTrabajo.IdOrden)
                return NotFound();

            var db = await _context.OrdenTrabajos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdOrden == id);

            if (db == null)
                return NotFound();

            // 🔒 BLOQUEO REAL
            if (db.Estado?.Trim().ToUpper() == "FINALIZADA")
                return BadRequest("No se puede editar una orden finalizada");

            if (ModelState.IsValid)
            {
                _context.Update(ordenTrabajo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarCombos(ordenTrabajo);
            return View(ordenTrabajo);
        }

        // =====================================================
        // DELETE (GET)
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ordenTrabajo = await _context.OrdenTrabajos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.IdMaquinaNavigation)
                .Include(o => o.IdTipoServicioNavigation)
                .Include(o => o.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdOrden == id);

            if (ordenTrabajo == null)
                return NotFound();

            return View(ordenTrabajo);
        }

        // =====================================================
        // DELETE (SOFT)
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordenTrabajo = await _context.OrdenTrabajos
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (ordenTrabajo == null)
                return NotFound();

            // 🔒 BLOQUEO REAL
            if (ordenTrabajo.Estado?.Trim().ToUpper() == "FINALIZADA")
                return BadRequest("No se puede cancelar una orden finalizada");

            ordenTrabajo.Estado = "CANCELADA";

            _context.Update(ordenTrabajo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // COMBOS
        // =====================================================
        private void CargarCombos(OrdenTrabajo? ordenTrabajo = null)
        {
            ViewData["IdCliente"] = new SelectList(
                _context.Clientes.Select(c => new
                {
                    c.IdCliente,
                    NombreCompleto = c.Nombres + " " + c.Apellidos
                }),
                "IdCliente",
                "NombreCompleto",
                ordenTrabajo?.IdCliente
            );

            ViewData["IdMaquina"] = new SelectList(
                _context.Maquinas.Select(m => new
                {
                    m.IdMaquina,
                    NombreMaquina = m.Marca + " - " + m.Modelo
                }),
                "IdMaquina",
                "NombreMaquina",
                ordenTrabajo?.IdMaquina
            );

            ViewData["IdTipoServicio"] = new SelectList(
                _context.TipoServicios,
                "IdTipoServicio",
                "Nombre",
                ordenTrabajo?.IdTipoServicio
            );

            ViewData["IdUsuario"] = new SelectList(
                _context.Usuarios.Select(u => new
                {
                    u.IdUsuario,
                    NombreUsuario = u.Nombre
                }),
                "IdUsuario",
                "NombreUsuario",
                ordenTrabajo?.IdUsuario
            );
        }

        private bool OrdenTrabajoExists(int id)
        {
            return _context.OrdenTrabajos.Any(e => e.IdOrden == id);
        }
    }
}