using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class OrdenTrabajo
{
    public int IdOrden { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public string? Estado { get; set; }

    public string? Descripcion { get; set; }

    public int? IdCliente { get; set; }

    public int? IdMaquina { get; set; }

    public int? IdTipoServicio { get; set; }

    public int? IdUsuario { get; set; }

    public virtual ICollection<Asignacion> Asignacions { get; set; } = new List<Asignacion>();

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Maquina? IdMaquinaNavigation { get; set; }

    public virtual TipoServicio? IdTipoServicioNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
