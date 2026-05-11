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
            var idRol = User.FindFirst("IdRol")?.Value;
            var idUsuario = User.FindFirst("IdUsuario")?.Value;

            var query = _context.OrdenTrabajos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.IdMaquinaNavigation)
                .Include(o => o.IdTipoServicioNavigation)
                .Include(o => o.Asignacions)
                    .ThenInclude(a => a.IdTecnicoNavigation)
                .AsQueryable();

            // =========================
            // TECNICO SOLO SUS ORDENES
            // =========================
            if (idRol == "2")
            {
                if (int.TryParse(idUsuario, out int userId))
                {
                    var idTecnico = _context.Tecnicos
                        .Where(t => t.id_usuario == userId)
                        .Select(t => t.IdTecnico)
                        .FirstOrDefault();

                    var idsOrdenes = _context.Asignacions
                        .Where(a => a.IdTecnico == idTecnico)
                        .Select(a => a.IdOrden);

                    query = query.Where(o => idsOrdenes.Contains(o.IdOrden));
                }
                else
                {
                    query = query.Where(o => false);
                }
            }

            // =========================
            // FILTRO ESTADO
            // =========================
            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(o => o.Estado == estado);
            }

            ViewBag.EstadoActual = estado;

            return View(await query.ToListAsync());
        }

        // =====================================================
        // DETAILS (🔥 CLAVE PARA REPUESTOS)
        // =====================================================
        public async Task<IActionResult> Details(int id)
        {
            var orden = await _context.OrdenTrabajos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.IdMaquinaNavigation)
                .Include(o => o.IdTipoServicioNavigation)
                .Include(o => o.IdUsuarioNavigation)

                // 🔥 ASIGNACIONES + REPUESTOS
                .Include(o => o.Asignacions)
                    .ThenInclude(a => a.SolicitudRepuestos)
                        .ThenInclude(s => s.IdRepuestoNavigation)

                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return RedirectToAction(nameof(Index));

            return View(orden);
        }

        // =====================================================
        // CREATE ORDEN
        // =====================================================
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrdenTrabajo orden)
        {
            if (ModelState.IsValid)
            {
                orden.Estado = "PENDIENTE";

                _context.Add(orden);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(orden);
            return View(orden);
        }

        // =====================================================
        // EDIT
        // =====================================================
        public async Task<IActionResult> Edit(int id)
        {
            var orden = await _context.OrdenTrabajos
                .Include(o => o.Asignacions)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound();

            if (!PuedeAcceder(orden))
                return Forbid();

            CargarCombos(orden);
            return View(orden);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, OrdenTrabajo ordenTrabajo)
        {
            var db = await _context.OrdenTrabajos.FindAsync(id);

            if (db == null)
                return NotFound();

            if (!PuedeAcceder(db))
                return Forbid();

            if (db.Estado?.ToUpper() == "FINALIZADA")
                return BadRequest("Orden finalizada");

            if (ModelState.IsValid)
            {
                db.Descripcion = ordenTrabajo.Descripcion;
                db.IdCliente = ordenTrabajo.IdCliente;
                db.IdMaquina = ordenTrabajo.IdMaquina;
                db.IdTipoServicio = ordenTrabajo.IdTipoServicio;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarCombos(ordenTrabajo);
            return View(ordenTrabajo);
        }

        // =====================================================
        // DELETE
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var orden = await _context.OrdenTrabajos
                .Include(o => o.Asignacions)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound();

            if (!PuedeAcceder(orden))
                return Forbid();

            orden.Estado = "CANCELADA";

            _context.Update(orden);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // SEGURIDAD
        // =====================================================
        private bool PuedeAcceder(OrdenTrabajo orden)
        {
            var idRol = User.FindFirst("IdRol")?.Value;
            var idUsuario = User.FindFirst("IdUsuario")?.Value;

            if (idRol == "1")
                return true;

            if (idRol == "2" && int.TryParse(idUsuario, out int userId))
            {
                var idTecnico = _context.Tecnicos
                    .Where(t => t.id_usuario == userId)
                    .Select(t => t.IdTecnico)
                    .FirstOrDefault();

                return orden.Asignacions.Any(a => a.IdTecnico == idTecnico);
            }

            return false;
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
        }
    }
}