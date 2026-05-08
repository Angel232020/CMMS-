using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class SolicitudRepuesto
{
    public int IdSolicitud { get; set; }

    public int? IdAsignacion { get; set; }

    public DateTime? Fecha { get; set; }

    public int? IdRepuesto { get; set; }

    public int? Cantidad { get; set; }

    public string? Comentarios { get; set; }

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual Asignacion? IdAsignacionNavigation { get; set; }

    public virtual Repuesto? IdRepuestoNavigation { get; set; }
}
