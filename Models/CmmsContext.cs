using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CMMS.Models;

public partial class CmmsContext : DbContext
{
    public CmmsContext()
    {
    }

    public CmmsContext(DbContextOptions<CmmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asignacion> Asignacions { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetalleOrden> DetalleOrdens { get; set; }

    public virtual DbSet<Maquina> Maquinas { get; set; }

    public virtual DbSet<OrdenTrabajo> OrdenTrabajos { get; set; }

    public virtual DbSet<Repuesto> Repuestos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<SolicitudRepuesto> SolicitudRepuestos { get; set; }

    public virtual DbSet<Tecnico> Tecnicos { get; set; }

    public virtual DbSet<TipoServicio> TipoServicios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-19G5AGQ\\SQLEXPRESS; Database=CMMS; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asignacion>(entity =>
        {
            entity.HasKey(e => e.IdAsignacion).HasName("PK__Asignaci__C3F7F966EA9C544A");

            entity.ToTable("Asignacion");

            entity.Property(e => e.IdAsignacion).HasColumnName("id_asignacion");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdTecnico).HasColumnName("id_tecnico");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Asignacions)
                .HasForeignKey(d => d.IdOrden)
                .HasConstraintName("FK__Asignacio__id_or__4CA06362");

            entity.HasOne(d => d.IdTecnicoNavigation).WithMany(p => p.Asignacions)
                .HasForeignKey(d => d.IdTecnico)
                .HasConstraintName("FK__Asignacio__id_te__4D94879B");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Asignacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Asignacio__id_us__4E88ABD4");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Cliente__677F38F5DF3AC508");

            entity.ToTable("Cliente");

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellidos");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<DetalleOrden>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__DetalleO__4F1332DEF6BC9943");

            entity.ToTable("DetalleOrden");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.CostoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("costo_total");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.IdAsignacion).HasColumnName("id_asignacion");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");

            entity.HasOne(d => d.IdAsignacionNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdAsignacion)
                .HasConstraintName("FK__DetalleOr__id_as__59063A47");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdOrden)
                .HasConstraintName("FK__DetalleOr__id_or__5812160E");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdSolicitud)
                .HasConstraintName("FK__DetalleOr__id_so__59FA5E80");
        });

        modelBuilder.Entity<Maquina>(entity =>
        {
            entity.HasKey(e => e.IdMaquina).HasName("PK__Maquina__9A2F321BB6683F5C");

            entity.ToTable("Maquina");

            entity.Property(e => e.IdMaquina).HasColumnName("id_maquina");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modelo");
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("serie");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Maquinas)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__Maquina__id_clie__3E52440B");
        });

        modelBuilder.Entity<OrdenTrabajo>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PK__OrdenTra__DD5B8F333A5D2AC2");

            entity.ToTable("OrdenTrabajo");

            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdMaquina).HasColumnName("id_maquina");
            entity.Property(e => e.IdTipoServicio).HasColumnName("id_tipo_servicio");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.OrdenTrabajos)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__OrdenTrab__id_cl__440B1D61");

            entity.HasOne(d => d.IdMaquinaNavigation).WithMany(p => p.OrdenTrabajos)
                .HasForeignKey(d => d.IdMaquina)
                .HasConstraintName("FK__OrdenTrab__id_ma__44FF419A");

            entity.HasOne(d => d.IdTipoServicioNavigation).WithMany(p => p.OrdenTrabajos)
                .HasForeignKey(d => d.IdTipoServicio)
                .HasConstraintName("FK__OrdenTrab__id_ti__45F365D3");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.OrdenTrabajos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__OrdenTrab__id_us__46E78A0C");
        });

        modelBuilder.Entity<Repuesto>(entity =>
        {
            entity.HasKey(e => e.IdRepuesto).HasName("PK__Repuesto__9D97D13F363F5A72");

            entity.ToTable("Repuesto");

            entity.Property(e => e.IdRepuesto).HasColumnName("id_repuesto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__6ABCB5E0384C04BE");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<SolicitudRepuesto>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__Solicitu__5C0C31F38ADB9FEE");

            entity.ToTable("SolicitudRepuesto");

            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Comentarios)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("comentarios");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdAsignacion).HasColumnName("id_asignacion");
            entity.Property(e => e.IdRepuesto).HasColumnName("id_repuesto");

            entity.HasOne(d => d.IdAsignacionNavigation).WithMany(p => p.SolicitudRepuestos)
                .HasForeignKey(d => d.IdAsignacion)
                .HasConstraintName("FK__Solicitud__id_as__5441852A");

            entity.HasOne(d => d.IdRepuestoNavigation).WithMany(p => p.SolicitudRepuestos)
                .HasForeignKey(d => d.IdRepuesto)
                .HasConstraintName("FK__Solicitud__id_re__5535A963");
        });

        modelBuilder.Entity<Tecnico>(entity =>
        {
            entity.HasKey(e => e.IdTecnico).HasName("PK__Tecnico__D550973730109881");

            entity.ToTable("Tecnico");

            entity.Property(e => e.IdTecnico).HasColumnName("id_tecnico");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellidos");
            entity.Property(e => e.Especialidad)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("especialidad");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<TipoServicio>(entity =>
        {
            entity.HasKey(e => e.IdTipoServicio).HasName("PK__TipoServ__4227AB8E23A0DEC1");

            entity.ToTable("TipoServicio");

            entity.Property(e => e.IdTipoServicio).HasColumnName("id_tipo_servicio");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__4E3E04ADB3462BE5");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__id_rol__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
