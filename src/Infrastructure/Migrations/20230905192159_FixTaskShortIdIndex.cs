using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTaskShortIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_ShortId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ShortId",
                table: "Tasks",
                column: "ShortId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_ShortId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ShortId",
                table: "Tasks",
                column: "ShortId",
                unique: true);
        }
    }
}
