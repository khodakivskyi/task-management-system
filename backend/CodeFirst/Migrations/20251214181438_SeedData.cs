using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Login", "Name", "PasswordHash", "Salt", "Surname" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe", "John", "hashed_password_1", "salt_1", "Doe" },
                    { 2, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith", "Jane", "hashed_password_2", "salt_2", "Smith" },
                    { 3, new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "bob.johnson", "Bob", "hashed_password_3", "salt_3", "Johnson" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Description", "EndDate", "Name", "OwnerId", "StartDate" },
                values: new object[,]
                {
                    { 1, "Complete redesign of company website with modern UI/UX", new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Website Redesign", 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Development of iOS and Android mobile application", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mobile App Development", 1, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Migration from legacy database to PostgreSQL", new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Database Migration", 2, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Integration with third-party payment and shipping APIs", new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "API Integration", 3, new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "CreatedAt", "Deadline", "Description", "EstimatedHours", "OwnerId", "Priority", "ProjectId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 7, new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Update API documentation with new endpoints", 8, 3, 1, null, "Update Documentation", new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 1, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Create wireframes and mockups for the new homepage design", 40, 1, 3, 1, "Design Homepage Layout", new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "ActualHours", "CreatedAt", "Deadline", "Description", "EstimatedHours", "OwnerId", "Priority", "ProjectId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, 8, new DateTime(2024, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Build responsive navigation menu with mobile hamburger", 24, 1, 2, 1, "Implement Responsive Navigation", new DateTime(2024, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 16, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Configure Xcode, CocoaPods, and development certificates", 16, 1, 3, 2, "Setup iOS Development Environment", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "CreatedAt", "Deadline", "Description", "EstimatedHours", "OwnerId", "Priority", "ProjectId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Create scripts to export all data from SQL Server database", 32, 2, 3, 3, "Export Data from Legacy Database", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Import exported data into new PostgreSQL database", 40, 2, 3, 3, "Import Data to PostgreSQL", new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "ActualHours", "CreatedAt", "Deadline", "Description", "EstimatedHours", "OwnerId", "Priority", "ProjectId", "Title", "UpdatedAt" },
                values: new object[] { 6, 12, new DateTime(2024, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Integrate Stripe payment gateway for processing payments", 48, 3, 3, 4, "Integrate Payment Gateway API", new DateTime(2024, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
