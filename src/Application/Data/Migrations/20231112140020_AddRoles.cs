using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "ProjectMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "OrganizationMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "OrganizationRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modifiable = table.Column<bool>(type: "bit", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationRoles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modifiable = table.Column<bool>(type: "bit", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRoles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRoles_OrganizationId",
                table: "OrganizationRoles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRoles_ProjectId",
                table: "ProjectRoles",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationRoles");

            migrationBuilder.DropTable(
                name: "ProjectRoles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "OrganizationMembers");
        }
    }
}
