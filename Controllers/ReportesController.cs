using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMMS.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ReporteController : Controller
{
    private readonly CmmsContext _context;

    public ReporteController(CmmsContext context)
    {
        _context = context;
    }

    public IActionResult Index(DateTime? fechaInicio, DateTime? fechaFin)
    {
         if (User.FindFirst("IdRol")?.Value != "1") // SOLO ADMIN
            return RedirectToAction("AccessDenied", "Home");

        var model = new Reportes();

        // 🔹 Valores por defecto
        fechaInicio ??= new DateTime(DateTime.Now.Year, 1, 1);
        fechaFin ??= DateTime.Now;

        DateTime fi = fechaInicio.Value.Date;
        DateTime ff = fechaFin.Value.Date;

        // =========================
        // KPIs
        // =========================

        model.TotalOrdenes = _context.OrdenTrabajos
            .Where(o => o.FechaCreacion != null &&
                        o.FechaCreacion.Value.Date >= fi &&
                        o.FechaCreacion.Value.Date <= ff)
            .Count();

        model.OrdenesPendientes = _context.OrdenTrabajos
            .Where(o => o.Estado == "PENDIENTE" &&
                        o.FechaCreacion != null &&
                        o.FechaCreacion.Value.Date >= fi &&
                        o.FechaCreacion.Value.Date <= ff)
            .Count();

        model.OrdenesFinalizadas = _context.OrdenTrabajos
            .Where(o => o.Estado == "FINALIZADA" &&
                        o.FechaCreacion != null &&
                        o.FechaCreacion.Value.Date >= fi &&
                        o.FechaCreacion.Value.Date <= ff)
            .Count();

            model.OrdenesCanceladas = _context.OrdenTrabajos
                        .Where(o => o.Estado == "CANCELADA" &&
                        o.FechaCreacion != null &&
                        o.FechaCreacion.Value.Date >= fi &&
                        o.FechaCreacion.Value.Date <= ff)
            .Count();

        // =========================
        // ESTADOS
        // =========================

        var estados = _context.OrdenTrabajos
            .Where(o => o.FechaCreacion != null &&
                        o.FechaCreacion.Value.Date >= fi &&
                        o.FechaCreacion.Value.Date <= ff)
            .GroupBy(o => o.Estado ?? "Sin estado")
            .Select(g => new { Estado = g.Key, Total = g.Count() })
            .ToList();

        model.Estados = estados.Select(e => e.Estado).ToList();
        model.CantidadEstados = estados.Select(e => e.Total).ToList();

        // =========================
        // TECNICOS
        // =========================

        var tecnicos = _context.Asignacions
            .Include(a => a.IdTecnicoNavigation)
            .Where(a => a.IdTecnicoNavigation != null &&
                        a.FechaAsignacion != null &&
                        a.FechaAsignacion.Value.Date >= fi &&
                        a.FechaAsignacion.Value.Date <= ff)
            .GroupBy(a =>
                (a.IdTecnicoNavigation != null
                    ? (a.IdTecnicoNavigation.Nombres ?? "") + " " + (a.IdTecnicoNavigation.Apellidos ?? "")
                    : "Sin técnico")
            )
            .Select(g => new { Tecnico = g.Key, Total = g.Count() })
            .ToList();

        model.Tecnicos = tecnicos.Select(t => t.Tecnico).ToList();
        model.TrabajosPorTecnico = tecnicos.Select(t => t.Total).ToList();

        // =========================
        // REPUESTOS
        // =========================

        var repuestos = _context.SolicitudRepuestos
            .Include(s => s.IdRepuestoNavigation)
            .Where(s => s.IdRepuestoNavigation != null &&
                        s.Fecha != null &&
                        s.Fecha.Value.Date >= fi &&
                        s.Fecha.Value.Date <= ff)
            .GroupBy(s => s.IdRepuestoNavigation.Nombre)
            .Select(g => new { Nombre = g.Key, Total = g.Sum(x => x.Cantidad) })
            .ToList();

        model.Repuestos = repuestos.Select(r => r.Nombre ?? "Sin nombre").ToList();
        model.UsoRepuestos = repuestos.Select(r => r.Total ?? 0).ToList();

        // =========================
        // INGRESOS POR MES
        // =========================

        var ingresos = _context.DetalleOrdens
            .Where(d => d.FechaFin != null &&
                        d.FechaFin.Value.Date >= fi &&
                        d.FechaFin.Value.Date <= ff)
            .GroupBy(d => new { d.FechaFin.Value.Year, d.FechaFin.Value.Month })
            .Select(g => new
            {
                Mes = g.Key.Month,
                Total = g.Sum(x => x.CostoTotal)
            })
            .OrderBy(x => x.Mes)
            .ToList();

        string[] meses = { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun",
                           "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

        model.Meses = ingresos.Select(x => meses[x.Mes]).ToList();
        model.IngresosPorMes = ingresos.Select(x => x.Total ?? 0).ToList();

        // =========================
        // VIEWBAG
        // =========================

        ViewBag.FechaInicio = fi.ToString("yyyy-MM-dd");
        ViewBag.FechaFin = ff.ToString("yyyy-MM-dd");

        return View(model);
    }

    
}