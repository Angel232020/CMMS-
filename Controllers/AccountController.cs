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
    public async Task<IActionResult> Login(string correo, string contrasena)
    {
        if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
        {
            ViewBag.Error = "Ingrese correo y contraseña";
            return View();
        }

        // 🔥 SOLO USUARIOS ACTIVOS
        var usuario = _context.Usuarios
            .FirstOrDefault(u => u.Correo == correo && u.Estado == true);

        if (usuario == null)
        {
            ViewBag.Error = "Usuario no existe o está inactivo";
            return View();
        }

        if (string.IsNullOrEmpty(usuario.Contrasena))
        {
            ViewBag.Error = "Usuario sin contraseña válida";
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

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

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
}