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
    public class MaquinasController : Controller
    {
        private readonly CmmsContext _context;

        public MaquinasController(CmmsContext context)
        {
            _context = context;
        }

        // GET: Maquinas
        public async Task<IActionResult> Index()
        {
            var cmmsContext = _context.Maquinas
                .Include(m => m.IdClienteNavigation);

            return View(await cmmsContext.ToListAsync());
        }

        // GET: Maquinas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maquina = await _context.Maquinas
                .Include(m => m.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdMaquina == id);

            if (maquina == null)
            {
                return NotFound();
            }

            return View(maquina);
        }

        // GET: Maquinas/Create
        public IActionResult Create()
        {
            ViewData["IdCliente"] = new SelectList(
                _context.Clientes.Select(c => new
                {
                    c.IdCliente,
                    NombreCompleto = c.Nombres + " " + c.Apellidos
                }),
                "IdCliente",
                "NombreCompleto"
            );

            return View();
        }

        // POST: Maquinas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdMaquina,Tipo,Marca,Modelo,Serie,IdCliente")] Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maquina);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCliente"] = new SelectList(
                _context.Clientes.Select(c => new
                {
                    c.IdCliente,
                    NombreCompleto = c.Nombres + " " + c.Apellidos
                }),
                "IdCliente",
                "NombreCompleto",
                maquina.IdCliente
            );

            return View(maquina);
        }

        // GET: Maquinas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maquina = await _context.Maquinas.FindAsync(id);

            if (maquina == null)
            {
                return NotFound();
            }

            ViewData["IdCliente"] = new SelectList(
                _context.Clientes.Select(c => new
                {
                    c.IdCliente,
                    NombreCompleto = c.Nombres + " " + c.Apellidos
                }),
                "IdCliente",
                "NombreCompleto",
                maquina.IdCliente
            );

            return View(maquina);
        }

        // POST: Maquinas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdMaquina,Tipo,Marca,Modelo,Serie,IdCliente")] Maquina maquina)
        {
            if (id != maquina.IdMaquina)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maquina);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaquinaExists(maquina.IdMaquina))
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

            ViewData["IdCliente"] = new SelectList(
                _context.Clientes.Select(c => new
                {
                    c.IdCliente,
                    NombreCompleto = c.Nombres + " " + c.Apellidos
                }),
                "IdCliente",
                "NombreCompleto",
                maquina.IdCliente
            );

            return View(maquina);
        }

        // GET: Maquinas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maquina = await _context.Maquinas
                .Include(m => m.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdMaquina == id);

            if (maquina == null)
            {
                return NotFound();
            }

            return View(maquina);
        }

        // POST: Maquinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maquina = await _context.Maquinas.FindAsync(id);

            if (maquina != null)
            {
                _context.Maquinas.Remove(maquina);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool MaquinaExists(int id)
        {
            return _context.Maquinas.Any(e => e.IdMaquina == id);
        }
    }
}