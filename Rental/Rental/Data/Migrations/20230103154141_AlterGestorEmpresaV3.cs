using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental.Data.Migrations
{
    public partial class AlterGestorEmpresaV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GestorId",
                table: "Empresa",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_GestorId",
                table: "Empresa",
                column: "GestorId",
                unique: true,
                filter: "[GestorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresa_AspNetUsers_GestorId",
                table: "Empresa",
                column: "GestorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresa_AspNetUsers_GestorId",
                table: "Empresa");

            migrationBuilder.DropIndex(
                name: "IX_Empresa_GestorId",
                table: "Empresa");

            migrationBuilder.AlterColumn<string>(
                name: "GestorId",
                table: "Empresa",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
