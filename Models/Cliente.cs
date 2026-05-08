using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public virtual ICollection<Maquina> Maquinas { get; set; } = new List<Maquina>();

    public virtual ICollection<OrdenTrabajo> OrdenTrabajos { get; set; } = new List<OrdenTrabajo>();
}
