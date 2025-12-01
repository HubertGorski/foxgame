using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoxTales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTermsAccepted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TermsAccepted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermsAcceptedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermsAccepted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TermsAcceptedAt",
                table: "Users");
        }
    }
}
