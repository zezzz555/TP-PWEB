using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental.Data.Migrations
{
    public partial class AlterEstadosVeioculos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KmTotais",
                table: "EstadoVeiculos");

            migrationBuilder.RenameColumn(
                name: "KmRealizados",
                table: "EstadoVeiculos",
                newName: "Kms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Kms",
                table: "EstadoVeiculos",
                newName: "KmRealizados");

            migrationBuilder.AddColumn<double>(
                name: "KmTotais",
                table: "EstadoVeiculos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
