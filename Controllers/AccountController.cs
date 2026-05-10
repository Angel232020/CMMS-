using CMMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

public class AccountController : Controller
{
    private readonly CmmsContext _context;
    private readonly PasswordHasher<Usuario> hasher = new PasswordHasher<Usuario>();

    public AccountController(CmmsContext context)
    {
        _context = context;
    }

    // =========================
    // LOGIN VIEW
    // =========================
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // =========================
    // LOGIN POST
    // =========================
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string correo, string contrasena)
    {
        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
        {
            ViewBag.Error = "Ingrese correo y contraseña";
            return View();
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuario == null)
        {
            ViewBag.Error = "Usuario no existe";
            return View();
        }

        if (!usuario.Estado)
        {
            ViewBag.Error = "Usuario inactivo";
            return View();
        }

        if (string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
            ViewBag.Error = "Usuario sin contraseña registrada";
            return View();
        }

        var resultado = hasher.VerifyHashedPassword(
            usuario,
            usuario.Contrasena,
            contrasena
        );

        if (resultado == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo ?? ""),
                new Claim("IdRol", usuario.IdRol?.ToString() ?? "0"),
                new Claim("IdUsuario", usuario.IdUsuario.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Index", "Menu");
        }

        ViewBag.Error = "Usuario o contraseña incorrectos";
        return View();
    }

    // =========================
    // LOGOUT
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    // =========================
    // CREATE USER
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Usuario usuario)
    {
        if (!ModelState.IsValid)
            return View(usuario);

        if (string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
            ModelState.AddModelError("Contrasena", "La contraseña es obligatoria");
            return View(usuario);
        }

        usuario.Contrasena = hasher.HashPassword(usuario, usuario.Contrasena);
        usuario.Estado = true;

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Usuarios");
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(
    Usuario usuario,
    [FromForm] string? nuevaContrasena)
{
    var userDb = await _context.Usuarios
        .FindAsync(usuario.IdUsuario);

    if (userDb == null)
        return NotFound();

    // ACTUALIZAR DATOS
    userDb.Nombre = usuario.Nombre;
    userDb.Correo = usuario.Correo;
    userDb.IdRol = usuario.IdRol;
    userDb.Estado = usuario.Estado;

    // ACTUALIZAR CONTRASEÑA SOLO SI ESCRIBIÓ UNA
    if (!string.IsNullOrWhiteSpace(nuevaContrasena))
    {
        userDb.Contrasena = hasher.HashPassword(userDb, nuevaContrasena);
    }

    await _context.SaveChangesAsync();

    return RedirectToAction("Index", "Usuarios");
}
}