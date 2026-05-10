using System;
using System.Collections.Generic;

namespace CMMS.Models;

public partial class Tecnico
{
    public int IdTecnico { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }

    public string? Especialidad { get; set; }

    public int? id_usuario { get; set; }

    public virtual ICollection<Asignacion> Asignacions { get; set; } = new List<Asignacion>();
}
