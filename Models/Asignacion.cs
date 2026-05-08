using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class Asignacion
{
    public int IdAsignacion { get; set; }

    public int? IdOrden { get; set; }

    public int? IdTecnico { get; set; }

    public DateTime? FechaAsignacion { get; set; }

    public int? IdUsuario { get; set; }

    // 🔥 NUEVO CAMPO PARA CANCELAR SIN BORRAR
    public string Estado { get; set; } = "ACTIVA";

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual OrdenTrabajo? IdOrdenNavigation { get; set; }

    public virtual Tecnico? IdTecnicoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<SolicitudRepuesto> SolicitudRepuestos { get; set; } = new List<SolicitudRepuesto>();
}