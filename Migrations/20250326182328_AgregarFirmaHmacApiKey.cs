using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABCNewsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFirmaHmacApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Key",
                table: "ApiKeys",
                newName: "FirmaHmac");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ApiKeys",
                newName: "Activo");

            migrationBuilder.AddColumn<string>(
                name: "Clave",
                table: "ApiKeys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clave",
                table: "ApiKeys");

            migrationBuilder.RenameColumn(
                name: "FirmaHmac",
                table: "ApiKeys",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "ApiKeys",
                newName: "IsActive");
        }
    }
}
