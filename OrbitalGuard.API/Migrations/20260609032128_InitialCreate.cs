using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrbitalGuard.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_REGIAO",
                columns: table => new
                {
                    ID_REGIAO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_REGIAO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    BIOMA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    AREA_KM2 = table.Column<decimal>(type: "DECIMAL(12,2)", precision: 12, scale: 2, nullable: true),
                    LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    RISCO_NIVEL = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_REGIAO", x => x.ID_REGIAO);
                });

            migrationBuilder.CreateTable(
                name: "TB_USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_USUARIO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    SENHA_HASH = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    PERFIL = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    ATIVO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DT_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "TB_ALERTA",
                columns: table => new
                {
                    ID_ALERTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TIPO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NIVEL = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    DT_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    ATIVO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ID_REGIAO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ALERTA", x => x.ID_ALERTA);
                    table.ForeignKey(
                        name: "FK_TB_ALERTA_TB_REGIAO_ID_REGIAO",
                        column: x => x.ID_REGIAO,
                        principalTable: "TB_REGIAO",
                        principalColumn: "ID_REGIAO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_USUARIO_REGIAO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_REGIAO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DT_INSCRICAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USUARIO_REGIAO", x => new { x.ID_USUARIO, x.ID_REGIAO });
                    table.ForeignKey(
                        name: "FK_TB_USUARIO_REGIAO_TB_REGIAO_ID_REGIAO",
                        column: x => x.ID_REGIAO,
                        principalTable: "TB_REGIAO",
                        principalColumn: "ID_REGIAO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_USUARIO_REGIAO_TB_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "TB_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_NOTIFICACAO",
                columns: table => new
                {
                    ID_NOTIFICACAO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MENSAGEM = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    CANAL = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    ENVIADA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DT_ENVIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_ALERTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_NOTIFICACAO", x => x.ID_NOTIFICACAO);
                    table.ForeignKey(
                        name: "FK_TB_NOTIFICACAO_TB_ALERTA_ID_ALERTA",
                        column: x => x.ID_ALERTA,
                        principalTable: "TB_ALERTA",
                        principalColumn: "ID_ALERTA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_NOTIFICACAO_TB_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "TB_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_ALERTA_ID_REGIAO",
                table: "TB_ALERTA",
                column: "ID_REGIAO");

            migrationBuilder.CreateIndex(
                name: "IX_TB_NOTIFICACAO_ID_ALERTA",
                table: "TB_NOTIFICACAO",
                column: "ID_ALERTA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_NOTIFICACAO_ID_USUARIO",
                table: "TB_NOTIFICACAO",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_TB_REGIAO_NM_REGIAO_ESTADO",
                table: "TB_REGIAO",
                columns: new[] { "NM_REGIAO", "ESTADO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_USUARIO_EMAIL",
                table: "TB_USUARIO",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_USUARIO_REGIAO_ID_REGIAO",
                table: "TB_USUARIO_REGIAO",
                column: "ID_REGIAO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_NOTIFICACAO");

            migrationBuilder.DropTable(
                name: "TB_USUARIO_REGIAO");

            migrationBuilder.DropTable(
                name: "TB_ALERTA");

            migrationBuilder.DropTable(
                name: "TB_USUARIO");

            migrationBuilder.DropTable(
                name: "TB_REGIAO");
        }
    }
}
