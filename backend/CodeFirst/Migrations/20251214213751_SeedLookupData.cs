using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class SeedLookupData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CanCreateTasks = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CanEditTasks = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CanDeleteTasks = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CanAssignTasks = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CanManageMembers = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMember_ProjectRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ProjectRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMember_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMember_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "Name" },
                values: new object[,]
                {
                    { 1, "#3498DB", "Development" },
                    { 2, "#9B59B6", "Design" },
                    { 3, "#E67E22", "Testing" },
                    { 4, "#1ABC9C", "Documentation" },
                    { 5, "#E74C3C", "Bug Fix" }
                });

            migrationBuilder.InsertData(
                table: "ProjectRoles",
                columns: new[] { "Id", "CanAssignTasks", "CanCreateTasks", "CanDeleteTasks", "CanEditTasks", "CanManageMembers", "Name" },
                values: new object[] { 1, true, true, true, true, true, "Owner" });

            migrationBuilder.InsertData(
                table: "ProjectRoles",
                columns: new[] { "Id", "CanAssignTasks", "CanCreateTasks", "CanDeleteTasks", "CanEditTasks", "Name" },
                values: new object[] { 2, true, true, true, true, "Manager" });

            migrationBuilder.InsertData(
                table: "ProjectRoles",
                columns: new[] { "Id", "CanCreateTasks", "CanEditTasks", "Name" },
                values: new object[] { 3, true, true, "Developer" });

            migrationBuilder.InsertData(
                table: "ProjectRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Viewer" });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "Color", "Name" },
                values: new object[,]
                {
                    { 1, "#FF6B6B", "To Do" },
                    { 2, "#4ECDC4", "In Progress" },
                    { 3, "#FFE66D", "In Review" },
                    { 4, "#95E1D3", "Done" },
                    { 5, "#C7C7C7", "Cancelled" }
                });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CategoryId",
                table: "Tasks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_ProjectId",
                table: "ProjectMember",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_RoleId",
                table: "ProjectMember",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_UserId",
                table: "ProjectMember",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Categories_CategoryId",
                table: "Tasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Statuses_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Categories_CategoryId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Statuses_StatusId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ProjectMember");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "ProjectRoles");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CategoryId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Tasks");
        }
    }
}
