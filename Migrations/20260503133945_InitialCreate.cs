using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMMS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombres = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    apellidos = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    direccion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cliente__677F38F5DF3AC508", x => x.id_cliente);
                });

            migrationBuilder.CreateTable(
                name: "Repuesto",
                columns: table => new
                {
                    id_repuesto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    stock = table.Column<int>(type: "int", nullable: true),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Repuesto__9D97D13F363F5A72", x => x.id_repuesto);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rol__6ABCB5E0384C04BE", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "Tecnico",
                columns: table => new
                {
                    id_tecnico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombres = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    apellidos = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    especialidad = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tecnico__D550973730109881", x => x.id_tecnico);
                });

            migrationBuilder.CreateTable(
                name: "TipoServicio",
                columns: table => new
                {
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoServ__4227AB8E23A0DEC1", x => x.id_tipo_servicio);
                });

            migrationBuilder.CreateTable(
                name: "Maquina",
                columns: table => new
                {
                    id_maquina = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    marca = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    modelo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    serie = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    id_cliente = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Maquina__9A2F321BB6683F5C", x => x.id_maquina);
                    table.ForeignKey(
                        name: "FK__Maquina__id_clie__3E52440B",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente");
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    contrasena = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    id_rol = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__4E3E04ADB3462BE5", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK__Usuario__id_rol__398D8EEE",
                        column: x => x.id_rol,
                        principalTable: "Rol",
                        principalColumn: "id_rol");
                });

            migrationBuilder.CreateTable(
                name: "OrdenTrabajo",
                columns: table => new
                {
                    id_orden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha_creacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    estado = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    id_cliente = table.Column<int>(type: "int", nullable: true),
                    id_maquina = table.Column<int>(type: "int", nullable: true),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: true),
                    id_usuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrdenTra__DD5B8F333A5D2AC2", x => x.id_orden);
                    table.ForeignKey(
                        name: "FK__OrdenTrab__id_cl__440B1D61",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente");
                    table.ForeignKey(
                        name: "FK__OrdenTrab__id_ma__44FF419A",
                        column: x => x.id_maquina,
                        principalTable: "Maquina",
                        principalColumn: "id_maquina");
                    table.ForeignKey(
                        name: "FK__OrdenTrab__id_ti__45F365D3",
                        column: x => x.id_tipo_servicio,
                        principalTable: "TipoServicio",
                        principalColumn: "id_tipo_servicio");
                    table.ForeignKey(
                        name: "FK__OrdenTrab__id_us__46E78A0C",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "Asignacion",
                columns: table => new
                {
                    id_asignacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_orden = table.Column<int>(type: "int", nullable: true),
                    id_tecnico = table.Column<int>(type: "int", nullable: true),
                    fecha_asignacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    id_usuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Asignaci__C3F7F966EA9C544A", x => x.id_asignacion);
                    table.ForeignKey(
                        name: "FK__Asignacio__id_or__4CA06362",
                        column: x => x.id_orden,
                        principalTable: "OrdenTrabajo",
                        principalColumn: "id_orden");
                    table.ForeignKey(
                        name: "FK__Asignacio__id_te__4D94879B",
                        column: x => x.id_tecnico,
                        principalTable: "Tecnico",
                        principalColumn: "id_tecnico");
                    table.ForeignKey(
                        name: "FK__Asignacio__id_us__4E88ABD4",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudRepuesto",
                columns: table => new
                {
                    id_solicitud = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_asignacion = table.Column<int>(type: "int", nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    id_repuesto = table.Column<int>(type: "int", nullable: true),
                    cantidad = table.Column<int>(type: "int", nullable: true),
                    comentarios = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Solicitu__5C0C31F38ADB9FEE", x => x.id_solicitud);
                    table.ForeignKey(
                        name: "FK__Solicitud__id_as__5441852A",
                        column: x => x.id_asignacion,
                        principalTable: "Asignacion",
                        principalColumn: "id_asignacion");
                    table.ForeignKey(
                        name: "FK__Solicitud__id_re__5535A963",
                        column: x => x.id_repuesto,
                        principalTable: "Repuesto",
                        principalColumn: "id_repuesto");
                });

            migrationBuilder.CreateTable(
                name: "DetalleOrden",
                columns: table => new
                {
                    id_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_orden = table.Column<int>(type: "int", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "datetime", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "datetime", nullable: true),
                    descripcion = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    estado = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    costo_total = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    id_asignacion = table.Column<int>(type: "int", nullable: true),
                    id_solicitud = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleO__4F1332DEF6BC9943", x => x.id_detalle);
                    table.ForeignKey(
                        name: "FK__DetalleOr__id_as__59063A47",
                        column: x => x.id_asignacion,
                        principalTable: "Asignacion",
                        principalColumn: "id_asignacion");
                    table.ForeignKey(
                        name: "FK__DetalleOr__id_or__5812160E",
                        column: x => x.id_orden,
                        principalTable: "OrdenTrabajo",
                        principalColumn: "id_orden");
                    table.ForeignKey(
                        name: "FK__DetalleOr__id_so__59FA5E80",
                        column: x => x.id_solicitud,
                        principalTable: "SolicitudRepuesto",
                        principalColumn: "id_solicitud");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asignacion_id_orden",
                table: "Asignacion",
                column: "id_orden");

            migrationBuilder.CreateIndex(
                name: "IX_Asignacion_id_tecnico",
                table: "Asignacion",
                column: "id_tecnico");

            migrationBuilder.CreateIndex(
                name: "IX_Asignacion_id_usuario",
                table: "Asignacion",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrden_id_asignacion",
                table: "DetalleOrden",
                column: "id_asignacion");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrden_id_orden",
                table: "DetalleOrden",
                column: "id_orden");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrden_id_solicitud",
                table: "DetalleOrden",
                column: "id_solicitud");

            migrationBuilder.CreateIndex(
                name: "IX_Maquina_id_cliente",
                table: "Maquina",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_id_cliente",
                table: "OrdenTrabajo",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_id_maquina",
                table: "OrdenTrabajo",
                column: "id_maquina");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_id_tipo_servicio",
                table: "OrdenTrabajo",
                column: "id_tipo_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_id_usuario",
                table: "OrdenTrabajo",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudRepuesto_id_asignacion",
                table: "SolicitudRepuesto",
                column: "id_asignacion");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudRepuesto_id_repuesto",
                table: "SolicitudRepuesto",
                column: "id_repuesto");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_id_rol",
                table: "Usuario",
                column: "id_rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleOrden");

            migrationBuilder.DropTable(
                name: "SolicitudRepuesto");

            migrationBuilder.DropTable(
                name: "Asignacion");

            migrationBuilder.DropTable(
                name: "Repuesto");

            migrationBuilder.DropTable(
                name: "OrdenTrabajo");

            migrationBuilder.DropTable(
                name: "Tecnico");

            migrationBuilder.DropTable(
                name: "Maquina");

            migrationBuilder.DropTable(
                name: "TipoServicio");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
