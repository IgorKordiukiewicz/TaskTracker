using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaskStatesManagerToWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskStates_TaskStatesManagers_TaskStatesManagerId",
                table: "TaskStates");

            migrationBuilder.DropTable(
                name: "TaskStatesManagers");

            migrationBuilder.RenameColumn(
                name: "TaskStatesManagerId",
                table: "TaskStates",
                newName: "WorkflowId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskStates_TaskStatesManagerId",
                table: "TaskStates",
                newName: "IX_TaskStates_WorkflowId");

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workflows_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_ProjectId",
                table: "Workflows",
                column: "ProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStates_Workflows_WorkflowId",
                table: "TaskStates",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskStates_Workflows_WorkflowId",
                table: "TaskStates");

            migrationBuilder.DropTable(
                name: "Workflows");

            migrationBuilder.RenameColumn(
                name: "WorkflowId",
                table: "TaskStates",
                newName: "TaskStatesManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskStates_WorkflowId",
                table: "TaskStates",
                newName: "IX_TaskStates_TaskStatesManagerId");

            migrationBuilder.CreateTable(
                name: "TaskStatesManagers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatesManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStatesManagers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatesManagers_ProjectId",
                table: "TaskStatesManagers",
                column: "ProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStates_TaskStatesManagers_TaskStatesManagerId",
                table: "TaskStates",
                column: "TaskStatesManagerId",
                principalTable: "TaskStatesManagers",
                principalColumn: "Id");
        }
    }
}
