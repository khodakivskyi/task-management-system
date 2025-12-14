using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Login = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    Salt = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DurationDays = table.Column<int>(type: "integer", nullable: true, computedColumnSql: "EXTRACT(DAY FROM (\"EndDate\" - \"StartDate\"))::integer", stored: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    Deadline = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EstimatedHours = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ActualHours = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "numeric", nullable: true, computedColumnSql: "CASE WHEN \"EstimatedHours\" > 0 THEN ROUND((\"ActualHours\"::numeric / \"EstimatedHours\"::numeric * 100.0), 2) ELSE 0.0 END", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.CheckConstraint("CK_Tasks_Priority", "\"Priority\" IS NULL OR (\"Priority\" >= 1 AND \"Priority\" <= 5)");
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    UploadedById = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAttachments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAttachments_Users_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                columns: new[] { "Id", "CreatedAt", "Deadline", "Details", "EstimatedHours", "OwnerId", "Priority", "ProjectId", "Status", "Tags", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 7, new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Update API documentation with new endpoints", 8, 3, 1, null, null, null, "Update Documentation", new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 1, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Create wireframes and mockups for the new homepage design", 40, 1, 3, 1, null, null, "Design Homepage Layout", new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "ActualHours", "CreatedAt", "Deadline", "Details", "EstimatedHours", "OwnerId", "Priority", "ProjectId", "Status", "Tags", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, 8, new DateTime(2024, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Build responsive navigation menu with mobile hamburger", 24, 1, 2, 1, null, null, "Implement Responsive Navigation", new DateTime(2024, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 16, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Configure Xcode, CocoaPods, and development certificates", 16, 1, 3, 2, null, null, "Setup iOS Development Environment", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "CreatedAt", "Deadline", "Details", "EstimatedHours", "OwnerId", "Priority", "ProjectId", "Status", "Tags", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Create scripts to export all data from SQL Server database", 32, 2, 3, 3, null, null, "Export Data from Legacy Database", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Import exported data into new PostgreSQL database", 40, 2, 3, 3, null, null, "Import Data to PostgreSQL", new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "ActualHours", "CreatedAt", "Deadline", "Details", "EstimatedHours", "OwnerId", "Priority", "ProjectId", "Status", "Tags", "Title", "UpdatedAt" },
                values: new object[] { 6, 12, new DateTime(2024, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Integrate Stripe payment gateway for processing payments", 48, 3, 3, 4, null, null, "Integrate Payment Gateway API", new DateTime(2024, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_EndDate",
                table: "Projects",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId_StartDate",
                table: "Projects",
                columns: new[] { "OwnerId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StartDate",
                table: "Projects",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StartDate_EndDate",
                table: "Projects",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_TaskId",
                table: "TaskAttachments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_TaskId_UploadedAt",
                table: "TaskAttachments",
                columns: new[] { "TaskId", "UploadedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_UploadedAt",
                table: "TaskAttachments",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_UploadedById",
                table: "TaskAttachments",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedAt",
                table: "Tasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Deadline",
                table: "Tasks",
                column: "Deadline",
                filter: "\"Deadline\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OwnerId",
                table: "Tasks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OwnerId_CreatedAt",
                table: "Tasks",
                columns: new[] { "OwnerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OwnerId_Priority_Deadline",
                table: "Tasks",
                columns: new[] { "OwnerId", "Priority", "Deadline" },
                filter: "\"Priority\" IS NOT NULL AND \"Deadline\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Priority",
                table: "Tasks",
                column: "Priority",
                filter: "\"Priority\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId",
                filter: "\"ProjectId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId_Deadline",
                table: "Tasks",
                columns: new[] { "ProjectId", "Deadline" },
                filter: "\"ProjectId\" IS NOT NULL AND \"Deadline\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Title",
                table: "Tasks",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UpdatedAt",
                table: "Tasks",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true,
                filter: "\"Login\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name_Surname",
                table: "Users",
                columns: new[] { "Name", "Surname" },
                filter: "\"Surname\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Surname",
                table: "Users",
                column: "Surname",
                filter: "\"Surname\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskAttachments");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
