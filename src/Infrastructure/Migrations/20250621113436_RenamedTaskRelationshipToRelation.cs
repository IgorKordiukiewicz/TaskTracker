using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedTaskRelationshipToRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskHierarchicalRelationships");

            migrationBuilder.DropTable(
                name: "TaskRelationshipManagers");

            migrationBuilder.CreateTable(
                name: "TaskRelationManagers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskRelationManagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskHierarchicalRelations",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TaskRelationManagerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHierarchicalRelations", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_TaskHierarchicalRelations_TaskRelationManagers_TaskRelation~",
                        column: x => x.TaskRelationManagerId,
                        principalTable: "TaskRelationManagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskHierarchicalRelations_Tasks_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskHierarchicalRelations_Tasks_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskHierarchicalRelations_ChildId",
                table: "TaskHierarchicalRelations",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHierarchicalRelations_TaskRelationManagerId",
                table: "TaskHierarchicalRelations",
                column: "TaskRelationManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskHierarchicalRelations");

            migrationBuilder.DropTable(
                name: "TaskRelationManagers");

            migrationBuilder.CreateTable(
                name: "TaskRelationshipManagers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskRelationshipManagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskHierarchicalRelationships",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TaskRelationshipManagerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHierarchicalRelationships", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_TaskHierarchicalRelationships_TaskRelationshipManagers_Task~",
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
    }
}
