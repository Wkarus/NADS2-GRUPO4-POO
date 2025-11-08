using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_PI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campanha",
                columns: table => new
                {
                    cd_campanha = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome_campanha = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    meta_arrecadacao = table.Column<decimal>(type: "REAL", nullable: true),
                    inicio = table.Column<DateTime>(type: "DATE", nullable: true),
                    fim = table.Column<DateTime>(type: "DATE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campanha", x => x.cd_campanha);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    cd_cliente = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome_completo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    cep = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    nome_usuario = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    senha = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.cd_cliente);
                });

            migrationBuilder.CreateTable(
                name: "Noticias",
                columns: table => new
                {
                    cd_noticias = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    cd_campanha = table.Column<int>(type: "INTEGER", nullable: false),
                    titulo_noticia = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    data_noticia = table.Column<DateTime>(type: "DATE", nullable: false),
                    autor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    conteudo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noticias", x => x.cd_noticias);
                    table.ForeignKey(
                        name: "FK_Noticias_Campanha_cd_campanha",
                        column: x => x.cd_campanha,
                        principalTable: "Campanha",
                        principalColumn: "cd_campanha",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Relatorio",
                columns: table => new
                {
                    cd_relatorio = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    cd_campanha = table.Column<int>(type: "INTEGER", nullable: false),
                    tipo_relatorio = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    valor_gasto = table.Column<decimal>(type: "REAL", nullable: true),
                    data_relatorio = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorio", x => x.cd_relatorio);
                    table.ForeignKey(
                        name: "FK_Relatorio_Campanha_cd_campanha",
                        column: x => x.cd_campanha,
                        principalTable: "Campanha",
                        principalColumn: "cd_campanha",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Doacao",
                columns: table => new
                {
                    cd_doacao = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    cd_cliente = table.Column<int>(type: "INTEGER", nullable: false),
                    cd_campanha = table.Column<int>(type: "INTEGER", nullable: false),
                    nome_doacao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    tipo_doacao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    forma_arrecadacao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    status_arrecadacao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doacao", x => x.cd_doacao);
                    table.ForeignKey(
                        name: "FK_Doacao_Campanha_cd_campanha",
                        column: x => x.cd_campanha,
                        principalTable: "Campanha",
                        principalColumn: "cd_campanha",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Doacao_Usuario_cd_cliente",
                        column: x => x.cd_cliente,
                        principalTable: "Usuario",
                        principalColumn: "cd_cliente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_doacao_status",
                table: "Doacao",
                column: "status_arrecadacao");

            migrationBuilder.CreateIndex(
                name: "IX_Doacao_cd_campanha",
                table: "Doacao",
                column: "cd_campanha");

            migrationBuilder.CreateIndex(
                name: "IX_Doacao_cd_cliente",
                table: "Doacao",
                column: "cd_cliente");

            migrationBuilder.CreateIndex(
                name: "idx_noticias_data",
                table: "Noticias",
                column: "data_noticia");

            migrationBuilder.CreateIndex(
                name: "IX_Noticias_cd_campanha",
                table: "Noticias",
                column: "cd_campanha");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorio_cd_campanha",
                table: "Relatorio",
                column: "cd_campanha");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_cpf",
                table: "Usuario",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_usuario_email",
                table: "Usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_nome_usuario",
                table: "Usuario",
                column: "nome_usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doacao");

            migrationBuilder.DropTable(
                name: "Noticias");

            migrationBuilder.DropTable(
                name: "Relatorio");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Campanha");
        }
    }
}
