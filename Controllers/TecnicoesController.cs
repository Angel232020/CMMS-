using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMMS.Models;

namespace CMMS.Controllers
{
    public class TecnicoesController : Controller
    {
        private readonly CmmsContext _context;

        public TecnicoesController(CmmsContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var tecnicos = await _context.Tecnicos
                .Include(t => t.IdTecnico)
                .ToListAsync();

            return View(tecnicos);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tecnico = await _context.Tecnicos
                .Include(t => t.IdTecnico)
                .FirstOrDefaultAsync(m => m.IdTecnico == id);

            if (tecnico == null)
            {
                return NotFound();
            }

            return View(tecnico);
        }

        // =========================
        // CREATE DESHABILITADO
        // =========================
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // CREATE POST DESHABILITADO
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tecnico tecnico)
        {
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT GET
        // =========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tecnico = await _context.Tecnicos
                .FindAsync(id);

            if (tecnico == null)
            {
                return NotFound();
            }

            return View(tecnico);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdTecnico,Nombres,Apellidos,Telefono,Especialidad,id_usuario")]
            Tecnico tecnico)
        {
            if (id != tecnico.IdTecnico)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tecnicoDb = await _context.Tecnicos
                        .FindAsync(id);

                    if (tecnicoDb == null)
                    {
                        return NotFound();
                    }

                    // ACTUALIZAR SOLO CAMPOS NECESARIOS
                    tecnicoDb.Nombres = tecnico.Nombres;
                    tecnicoDb.Apellidos = tecnico.Apellidos;
                    tecnicoDb.Telefono = tecnico.Telefono;
                    tecnicoDb.Especialidad = tecnico.Especialidad;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TecnicoExists(tecnico.IdTecnico))
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

            return View(tecnico);
        }

        // =========================
        // DELETE GET
        // =========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tecnico = await _context.Tecnicos
                .Include(t => t.IdTecnico)
                .FirstOrDefaultAsync(m => m.IdTecnico == id);

            if (tecnico == null)
            {
                return NotFound();
            }

            return View(tecnico);
        }

        // =========================
        // DELETE POST
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tecnico = await _context.Tecnicos
                .FindAsync(id);

            if (tecnico != null)
            {
                _context.Tecnicos.Remove(tecnico);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EXISTS
        // =========================
        private bool TecnicoExists(int id)
        {
            return _context.Tecnicos
                .Any(e => e.IdTecnico == id);
        }
    }
}