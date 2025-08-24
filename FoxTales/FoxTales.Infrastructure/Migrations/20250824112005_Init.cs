using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoxTales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionsIds",
                table: "Catalogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionsIds",
                table: "Catalogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
