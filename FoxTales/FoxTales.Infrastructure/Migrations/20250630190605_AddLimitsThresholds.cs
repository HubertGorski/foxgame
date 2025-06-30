using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoxTales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLimitsThresholds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLimits",
                table: "UserLimits");

            migrationBuilder.DropIndex(
                name: "IX_UserLimits_UserId",
                table: "UserLimits");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserLimits");

            migrationBuilder.DropColumn(
                name: "LimitName",
                table: "UserLimits");

            migrationBuilder.AddColumn<int>(
                name: "LimitId",
                table: "UserLimits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLimits",
                table: "UserLimits",
                columns: new[] { "UserId", "Type", "LimitId" });

            migrationBuilder.CreateTable(
                name: "LimitDefinitions",
                columns: table => new
                {
                    Type = table.Column<int>(type: "int", nullable: false),
                    LimitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDefinitions", x => new { x.Type, x.LimitId });
                });

            migrationBuilder.CreateTable(
                name: "LimitThresholds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    LimitId = table.Column<int>(type: "int", nullable: false),
                    ThresholdValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitThresholds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LimitThresholds_LimitDefinitions_Type_LimitId",
                        columns: x => new { x.Type, x.LimitId },
                        principalTable: "LimitDefinitions",
                        principalColumns: new[] { "Type", "LimitId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLimits_Type_LimitId",
                table: "UserLimits",
                columns: new[] { "Type", "LimitId" });

            migrationBuilder.CreateIndex(
                name: "IX_LimitThresholds_Type_LimitId",
                table: "LimitThresholds",
                columns: new[] { "Type", "LimitId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserLimits_LimitDefinitions_Type_LimitId",
                table: "UserLimits",
                columns: new[] { "Type", "LimitId" },
                principalTable: "LimitDefinitions",
                principalColumns: new[] { "Type", "LimitId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLimits_LimitDefinitions_Type_LimitId",
                table: "UserLimits");

            migrationBuilder.DropTable(
                name: "LimitThresholds");

            migrationBuilder.DropTable(
                name: "LimitDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLimits",
                table: "UserLimits");

            migrationBuilder.DropIndex(
                name: "IX_UserLimits_Type_LimitId",
                table: "UserLimits");

            migrationBuilder.DropColumn(
                name: "LimitId",
                table: "UserLimits");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserLimits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "LimitName",
                table: "UserLimits",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLimits",
                table: "UserLimits",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserLimits_UserId",
                table: "UserLimits",
                column: "UserId");
        }
    }
}
