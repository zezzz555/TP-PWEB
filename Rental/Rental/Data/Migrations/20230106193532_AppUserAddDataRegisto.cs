using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental.Data.Migrations
{
    public partial class AppUserAddDataRegisto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deposito",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "LocalLevantamento",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "Mala",
                table: "Veiculo");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRegisto",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataRegisto",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Deposito",
                table: "Veiculo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalLevantamento",
                table: "Veiculo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mala",
                table: "Veiculo",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
