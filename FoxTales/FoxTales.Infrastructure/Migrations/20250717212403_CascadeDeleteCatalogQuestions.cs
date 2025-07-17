using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoxTales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteCatalogQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogQuestions_Questions_QuestionId",
                table: "CatalogQuestions");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogQuestions_Questions_QuestionId",
                table: "CatalogQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogQuestions_Questions_QuestionId",
                table: "CatalogQuestions");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogQuestions_Questions_QuestionId",
                table: "CatalogQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
