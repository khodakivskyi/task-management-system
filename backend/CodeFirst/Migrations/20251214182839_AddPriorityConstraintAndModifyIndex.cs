using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityConstraintAndModifyIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_OwnerId_Priority",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OwnerId_Priority_Deadline",
                table: "Tasks",
                columns: new[] { "OwnerId", "Priority", "Deadline" },
                filter: "\"Priority\" IS NOT NULL AND \"Deadline\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Tasks_Priority",
                table: "Tasks",
                sql: "\"Priority\" IS NULL OR (\"Priority\" >= 1 AND \"Priority\" <= 5)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_OwnerId_Priority_Deadline",
                table: "Tasks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Tasks_Priority",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OwnerId_Priority",
                table: "Tasks",
                columns: new[] { "OwnerId", "Priority" },
                filter: "\"Priority\" IS NOT NULL");
        }
    }
}
