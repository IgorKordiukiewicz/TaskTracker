using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestampsToOrganizationInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrganizationInvitations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalizedAt",
                table: "OrganizationInvitations",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "FinalizedAt",
                table: "OrganizationInvitations");
        }
    }
}
