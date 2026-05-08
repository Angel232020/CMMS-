using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class Repuesto
{
    public int IdRepuesto { get; set; }

    public string? Nombre { get; set; }

    public int? Stock { get; set; }

    public decimal? Precio { get; set; }

    public virtual ICollection<SolicitudRepuesto> SolicitudRepuestos { get; set; } = new List<SolicitudRepuesto>();
}
