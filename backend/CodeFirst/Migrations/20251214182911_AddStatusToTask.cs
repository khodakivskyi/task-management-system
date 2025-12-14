using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Custom SQL for data transformation: Populate Status based on Priority
            // Priority 1 = Low, 2 = Medium, 3 = High, 4 = Critical, 5 = Urgent
            migrationBuilder.Sql(@"
                UPDATE ""Tasks""
                SET ""Status"" = CASE
                    WHEN ""Priority"" = 1 THEN 'Low'
                    WHEN ""Priority"" = 2 THEN 'Medium'
                    WHEN ""Priority"" = 3 THEN 'High'
                    WHEN ""Priority"" = 4 THEN 'Critical'
                    WHEN ""Priority"" = 5 THEN 'Urgent'
                    ELSE 'Not Set'
                END
                WHERE ""Priority"" IS NOT NULL;

                -- Set default status for tasks without priority
                UPDATE ""Tasks""
                SET ""Status"" = 'Not Set'
                WHERE ""Priority"" IS NULL AND ""Status"" IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tasks");
        }
    }
}
