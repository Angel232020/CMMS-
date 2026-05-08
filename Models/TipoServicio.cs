using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class TipoServicio
{
    public int IdTipoServicio { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<OrdenTrabajo> OrdenTrabajos { get; set; } = new List<OrdenTrabajo>();
}
