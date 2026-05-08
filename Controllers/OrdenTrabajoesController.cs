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
    public class OrdenTrabajoesController : Controller
    {
        private readonly CmmsContext _context;

        public OrdenTrabajoesController(CmmsContext context)
        {
            _context = context;
        }

        // GET: Index
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

        // GET: Details
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

        // GET: Create
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdOrden,FechaCreacion,Estado,Descripcion,IdCliente,IdMaquina,IdTipoServicio,IdUsuario")]
            OrdenTrabajo ordenTrabajo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordenTrabajo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarCombos(ordenTrabajo);
            return View(ordenTrabajo);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ordenTrabajo = await _context.OrdenTrabajos.FindAsync(id);

            if (ordenTrabajo == null)
                return NotFound();

            CargarCombos(ordenTrabajo);
            return View(ordenTrabajo);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdOrden,FechaCreacion,Estado,Descripcion,IdCliente,IdMaquina,IdTipoServicio,IdUsuario")]
            OrdenTrabajo ordenTrabajo)
        {
            if (id != ordenTrabajo.IdOrden)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordenTrabajo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdenTrabajoExists(ordenTrabajo.IdOrden))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            CargarCombos(ordenTrabajo);
            return View(ordenTrabajo);
        }

        // GET: Delete (solo confirmación)
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

        // POST: CANCELAR (SOFT DELETE REAL)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordenTrabajo = await _context.OrdenTrabajos
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (ordenTrabajo == null)
                return NotFound();

            // 🔥 NO SE BORRA - SOLO SE CANCELA
            ordenTrabajo.Estado = "Cancelada";

            _context.Entry(ordenTrabajo).Property(x => x.Estado).IsModified = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // 🔧 MÉTODOS AUXILIARES
        // ===============================

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