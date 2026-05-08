using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class Maquina
{
    public int IdMaquina { get; set; }

    public string? Tipo { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Serie { get; set; }

    public int? IdCliente { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual ICollection<OrdenTrabajo> OrdenTrabajos { get; set; } = new List<OrdenTrabajo>();
}
