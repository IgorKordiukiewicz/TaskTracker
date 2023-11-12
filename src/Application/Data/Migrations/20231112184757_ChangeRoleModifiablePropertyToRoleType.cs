using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRoleModifiablePropertyToRoleType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modifiable",
                table: "ProjectRoles");

            migrationBuilder.DropColumn(
                name: "Modifiable",
                table: "OrganizationRoles");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ProjectRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "OrganizationRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProjectRoles");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "OrganizationRoles");

            migrationBuilder.AddColumn<bool>(
                name: "Modifiable",
                table: "ProjectRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Modifiable",
                table: "OrganizationRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
