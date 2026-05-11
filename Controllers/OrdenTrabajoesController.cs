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
            var query = _context.OrdenTrabajos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.IdMaquinaNavigation)
                .Include(o => o.IdTipoServicioNavigation)
                .Include(o => o.IdUsuarioNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(o => o.Estado == estado);
            }

            ViewBag.EstadoActual = estado;

            return View(await query.ToListAsync());
        }

        // =====================================================
        // DETAILS
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var orden = await _context.OrdenTrabajos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.IdMaquinaNavigation)
                .Include(o => o.IdTipoServicioNavigation)
                .Include(o => o.IdUsuarioNavigation)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound();

            return View(orden);
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
        public async Task<IActionResult> Create(OrdenTrabajo orden)
        {
            if (ModelState.IsValid)
            {
                // 🔒 SIEMPRE PENDIENTE AL CREAR
                orden.Estado = "PENDIENTE";

                _context.Add(orden);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(orden);
            return View(orden);
        }

        // =====================================================
        // EDIT GET
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var orden = await _context.OrdenTrabajos.FindAsync(id);

            if (orden == null)
                return NotFound();

            // 🔒 BLOQUEO FINALIZADA
            if (orden.Estado?.Trim().ToUpper() == "FINALIZADA")
                return RedirectToAction(nameof(Index));

            CargarCombos(orden);
            return View(orden);
        }

        // =====================================================
        // EDIT POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrdenTrabajo ordenTrabajo)
        {
            if (id != ordenTrabajo.IdOrden)
                return NotFound();

            var db = await _context.OrdenTrabajos
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (db == null)
                return NotFound();

            // 🔒 BLOQUEO REAL FINALIZADA
            if (db.Estado?.Trim().ToUpper() == "FINALIZADA")
                return BadRequest("No se puede editar una orden finalizada");

            if (ModelState.IsValid)
            {
                db.Descripcion = ordenTrabajo.Descripcion;
                db.IdCliente = ordenTrabajo.IdCliente;
                db.IdMaquina = ordenTrabajo.IdMaquina;
                db.IdTipoServicio = ordenTrabajo.IdTipoServicio;
                db.IdUsuario = ordenTrabajo.IdUsuario;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(ordenTrabajo);
            return View(ordenTrabajo);
        }

        // =====================================================
        // DELETE GET
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var orden = await _context.OrdenTrabajos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.IdMaquinaNavigation)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound();

            return View(orden);
        }

        // =====================================================
        // DELETE POST (SOFT DELETE)
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orden = await _context.OrdenTrabajos.FindAsync(id);

            if (orden == null)
                return NotFound();

            // 🔒 BLOQUEO FINALIZADA
            if (orden.Estado?.Trim().ToUpper() == "FINALIZADA")
                return BadRequest("No se puede eliminar una orden finalizada");

            orden.Estado = "CANCELADA";

            _context.Update(orden);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // COMBOS
        // =====================================================
        private void CargarCombos(OrdenTrabajo? orden = null)
        {
            ViewData["IdCliente"] = new SelectList(
                _context.Clientes.Select(c => new
                {
                    c.IdCliente,
                    Nombre = c.Nombres + " " + c.Apellidos
                }),
                "IdCliente",
                "Nombre",
                orden?.IdCliente
            );

            ViewData["IdMaquina"] = new SelectList(
                _context.Maquinas.Select(m => new
                {
                    m.IdMaquina,
                    Nombre = m.Marca + " - " + m.Modelo
                }),
                "IdMaquina",
                "Nombre",
                orden?.IdMaquina
            );

            ViewData["IdTipoServicio"] = new SelectList(
                _context.TipoServicios,
                "IdTipoServicio",
                "Nombre",
                orden?.IdTipoServicio
            );

            ViewData["IdUsuario"] = new SelectList(
                _context.Usuarios,
                "IdUsuario",
                "Nombre",
                orden?.IdUsuario
            );
        }

        // =====================================================
        // EXISTS
        // =====================================================
        private bool OrdenTrabajoExists(int id)
        {
            return _context.OrdenTrabajos.Any(e => e.IdOrden == id);
        }
    }
}