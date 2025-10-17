using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoxTales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSeeders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogQuestions_Catalogs_CatalogId",
                table: "CatalogQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_Users_OwnerId",
                table: "Catalogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_OwnerId",
                table: "Questions");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Catalogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogQuestions_Catalogs_CatalogId",
                table: "CatalogQuestions",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "CatalogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Users_OwnerId",
                table: "Catalogs",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_OwnerId",
                table: "Questions",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogQuestions_Catalogs_CatalogId",
                table: "CatalogQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_Users_OwnerId",
                table: "Catalogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_OwnerId",
                table: "Questions");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Catalogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogQuestions_Catalogs_CatalogId",
                table: "CatalogQuestions",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "CatalogId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Users_OwnerId",
                table: "Catalogs",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_OwnerId",
                table: "Questions",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
