using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskStatusTransition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PossibleNextStatuses",
                table: "TaskStatuses");

            migrationBuilder.RenameColumn(
                name: "IsInitial",
                table: "TaskStatuses",
                newName: "Initial");

            migrationBuilder.CreateTable(
                name: "TaskStatusTransitions",
                columns: table => new
                {
                    FromStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatusTransitions", x => new { x.FromStatusId, x.ToStatusId });
                    table.ForeignKey(
                        name: "FK_TaskStatusTransitions_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusTransitions_WorkflowId",
                table: "TaskStatusTransitions",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskStatusTransitions");

            migrationBuilder.RenameColumn(
                name: "Initial",
                table: "TaskStatuses",
                newName: "IsInitial");

            migrationBuilder.AddColumn<string>(
                name: "PossibleNextStatuses",
                table: "TaskStatuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
