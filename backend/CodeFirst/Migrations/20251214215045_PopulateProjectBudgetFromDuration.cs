using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class PopulateProjectBudgetFromDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Custom SQL: Populate Budget based on DurationDays
            // Formula: Budget = DurationDays * 1000 (if Budget is NULL and DurationDays is not NULL)
            migrationBuilder.Sql(@"
                UPDATE ""Projects""
                SET ""Budget"" = ""DurationDays"" * 1000.0
                WHERE ""Budget"" IS NULL 
                  AND ""DurationDays"" IS NOT NULL 
                  AND ""DurationDays"" > 0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Set Budget to NULL for projects that were populated by this migration
            // Note: This is a best-effort rollback - we can't perfectly identify which budgets
            // were set by this migration vs manually set
            migrationBuilder.Sql(@"
                UPDATE ""Projects""
                SET ""Budget"" = NULL
                WHERE ""Budget"" = ""DurationDays"" * 1000.0
                  AND ""DurationDays"" IS NOT NULL;
            ");
        }
    }
}
