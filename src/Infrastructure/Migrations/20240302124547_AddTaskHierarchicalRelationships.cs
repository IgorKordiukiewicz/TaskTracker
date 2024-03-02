using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskHierarchicalRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskRelationshipManagers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskRelationshipManagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskHierarchicalRelationships",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TaskRelationshipManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHierarchicalRelationships", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_TaskHierarchicalRelationships_TaskRelationshipManagers_TaskRelationshipManagerId",
                        column: x => x.TaskRelationshipManagerId,
                        principalTable: "TaskRelationshipManagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskHierarchicalRelationships_Tasks_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskHierarchicalRelationships_Tasks_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskHierarchicalRelationships_ChildId",
                table: "TaskHierarchicalRelationships",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHierarchicalRelationships_TaskRelationshipManagerId",
                table: "TaskHierarchicalRelationships",
                column: "TaskRelationshipManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskHierarchicalRelationships");

            migrationBuilder.DropTable(
                name: "TaskRelationshipManagers");
        }
    }
}
