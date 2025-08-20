using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Migrations
{
    /// <inheritdoc />
    public partial class Indices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
       name: "IX_Bill_EmitterCompanyId_Letra_PV_Numero",  // Nombre del índice
       table: "Bills",  // Nombre de la tabla
       columns: new[] { "EmitterCompanyId", "Letra", "PV", "Numero" },  // Columnas del índice
       unique: true);  // Para hacerlo único
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
