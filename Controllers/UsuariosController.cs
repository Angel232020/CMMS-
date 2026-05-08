using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMMS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public class UsuariosController : Controller
{
    private readonly CmmsContext _context;

    public UsuariosController(CmmsContext context)
    {
        _context = context;
    }

    // LISTA
    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .ToListAsync();

        return View(usuarios);
    }

    // DETALLE
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var user = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(x => x.IdUsuario == id);

        if (user == null) return NotFound();

        return View(user);
    }

    // CREATE GET
    public IActionResult Create()
    {
        ViewData["Roles"] = new SelectList(_context.Rols, "IdRol", "Nombre");
        return View();
    }

    [HttpPost]
public async Task<IActionResult> Create(Usuario usuario)
{
    if (ModelState.IsValid)
    {
                if (string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
            ModelState.AddModelError("Contrasena", "La contraseña es obligatoria");
            return View(usuario);
        }

        var hasher = new PasswordHasher<Usuario>();
        usuario.Contrasena = hasher.HashPassword(usuario, usuario.Contrasena);
        usuario.Estado = true;

        _context.Add(usuario);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    ViewData["Roles"] = new SelectList(_context.Rols, "IdRol", "Nombre");
    return View(usuario);
}

    // EDIT GET
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var user = await _context.Usuarios.FindAsync(id);

        if (user == null) return NotFound();

        ViewData["Roles"] = new SelectList(_context.Rols, "IdRol", "Nombre", user.IdRol);

        return View(user);
    }

    // EDIT POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Usuario usuario)
    {
        if (id != usuario.IdUsuario) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(usuario);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(e => e.IdUsuario == usuario.IdUsuario))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["Roles"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
        return View(usuario);
    }

    // DESACTIVAR (NO BORRAR)
    public async Task<IActionResult> Disable(int id)
    {
        var user = await _context.Usuarios.FindAsync(id);

        if (user != null)
        {
            user.Estado = false;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}