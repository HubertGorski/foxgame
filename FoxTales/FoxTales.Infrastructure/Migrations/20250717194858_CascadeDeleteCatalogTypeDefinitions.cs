using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoxTales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteCatalogTypeDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogTypeDefinitions_CatalogTypes_CatalogTypeId",
                table: "CatalogTypeDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogTypeDefinitions_Catalogs_CatalogId",
                table: "CatalogTypeDefinitions");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogTypeDefinitions_CatalogTypes_CatalogTypeId",
                table: "CatalogTypeDefinitions",
                column: "CatalogTypeId",
                principalTable: "CatalogTypes",
                principalColumn: "CatalogTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogTypeDefinitions_Catalogs_CatalogId",
                table: "CatalogTypeDefinitions",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "CatalogId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogTypeDefinitions_CatalogTypes_CatalogTypeId",
                table: "CatalogTypeDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogTypeDefinitions_Catalogs_CatalogId",
                table: "CatalogTypeDefinitions");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogTypeDefinitions_CatalogTypes_CatalogTypeId",
                table: "CatalogTypeDefinitions",
                column: "CatalogTypeId",
                principalTable: "CatalogTypes",
                principalColumn: "CatalogTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogTypeDefinitions_Catalogs_CatalogId",
                table: "CatalogTypeDefinitions",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "CatalogId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
