using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoxTales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogTypes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatalogTypeId",
                table: "Catalogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_CatalogTypeId",
                table: "Catalogs",
                column: "CatalogTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_CatalogTypes_CatalogTypeId",
                table: "Catalogs",
                column: "CatalogTypeId",
                principalTable: "CatalogTypes",
                principalColumn: "CatalogTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_CatalogTypes_CatalogTypeId",
                table: "Catalogs");

            migrationBuilder.DropIndex(
                name: "IX_Catalogs_CatalogTypeId",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "CatalogTypeId",
                table: "Catalogs");
        }
    }
}
