using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class DetalleOrden
{
    public int IdDetalle { get; set; }

    public int? IdOrden { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string? Descripcion { get; set; }

    public string? Estado { get; set; }

    public decimal? CostoTotal { get; set; }

    public int? IdAsignacion { get; set; }

    public int? IdSolicitud { get; set; }

    public virtual Asignacion? IdAsignacionNavigation { get; set; }

    public virtual OrdenTrabajo? IdOrdenNavigation { get; set; }

    public virtual SolicitudRepuesto? IdSolicitudNavigation { get; set; }
}
