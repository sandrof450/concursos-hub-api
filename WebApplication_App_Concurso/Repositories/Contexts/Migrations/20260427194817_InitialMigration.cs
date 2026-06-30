using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_App_Concurso.Repositories.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Concursos",
                columns: table => new
                {
                    ConcursoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Orgao = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<string>(type: "text", nullable: true),
                    Vagas = table.Column<int>(type: "integer", nullable: true),
                    Nivel = table.Column<string>(type: "text", nullable: true),
                    Salario = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Fonte = table.Column<string>(type: "text", nullable: true),
                    DataPublicacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concursos", x => x.ConcursoId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Concursos");
        }
    }
}
