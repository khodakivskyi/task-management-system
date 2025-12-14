using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetConstraintAndModifyIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId_StartDate_Budget",
                table: "Projects",
                columns: new[] { "OwnerId", "StartDate", "Budget" },
                filter: "\"Budget\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Projects_Budget",
                table: "Projects",
                sql: "\"Budget\" IS NULL OR (\"Budget\" >= 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_OwnerId_StartDate_Budget",
                table: "Projects");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Projects_Budget",
                table: "Projects");
        }
    }
}
