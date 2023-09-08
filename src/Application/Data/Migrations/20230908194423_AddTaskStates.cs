using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StateId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.CreateTable(
                name: "TaskStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInitial = table.Column<bool>(type: "bit", nullable: false),
                    PossibleNextStates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskStatesManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStates_TaskStatesManagers_TaskStatesManagerId",
                        column: x => x.TaskStatesManagerId,
                        principalTable: "TaskStatesManagers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StateId",
                table: "Tasks",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStates_TaskStatesManagerId",
                table: "TaskStates",
                column: "TaskStatesManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatesManagers_ProjectId",
                table: "TaskStatesManagers",
                column: "ProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStates_StateId",
                table: "Tasks",
                column: "StateId",
                principalTable: "TaskStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStates_StateId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskStates");

            migrationBuilder.DropTable(
                name: "TaskStatesManagers");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_StateId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Tasks");
        }
    }
}
