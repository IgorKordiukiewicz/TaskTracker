using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaskStateToTaskStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStates_StateId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskStates");

            migrationBuilder.RenameColumn(
                name: "StateId",
                table: "Tasks",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_StateId",
                table: "Tasks",
                newName: "IX_Tasks_StatusId");

            migrationBuilder.CreateTable(
                name: "TaskStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInitial = table.Column<bool>(type: "bit", nullable: false),
                    PossibleNextStatuses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStatuses_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatuses_WorkflowId",
                table: "TaskStatuses",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStatuses_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "TaskStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStatuses_StatusId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskStatuses");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Tasks",
                newName: "StateId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks",
                newName: "IX_Tasks_StateId");

            migrationBuilder.CreateTable(
                name: "TaskStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsInitial = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PossibleNextStates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStates_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStates_WorkflowId",
                table: "TaskStates",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStates_StateId",
                table: "Tasks",
                column: "StateId",
                principalTable: "TaskStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
