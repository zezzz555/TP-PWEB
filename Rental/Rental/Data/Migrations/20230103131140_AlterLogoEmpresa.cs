using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental.Data.Migrations
{
    public partial class AlterLogoEmpresa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoURL",
                table: "Empresa");

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "Empresa",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Empresa");

            migrationBuilder.AddColumn<string>(
                name: "LogoURL",
                table: "Empresa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
