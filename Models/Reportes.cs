using System.Collections.Generic;

public class Reportes
{
    public int TotalOrdenes { get; set; }
    public int OrdenesPendientes { get; set; }
    public int OrdenesFinalizadas { get; set; }
    public decimal CostoTotal { get; set; }

    public List<string> Estados { get; set; } = new();
    public List<int> CantidadEstados { get; set; } = new();

    public List<string> Tecnicos { get; set; } = new();
    public List<int> TrabajosPorTecnico { get; set; } = new();

    public List<string> Repuestos { get; set; } = new();
    public List<int> UsoRepuestos { get; set; } = new();

    public List<string> Meses { get; set; } = new();
    public List<decimal> IngresosPorMes { get; set; } = new();
    public int OrdenesCanceladas { get; set; }
}