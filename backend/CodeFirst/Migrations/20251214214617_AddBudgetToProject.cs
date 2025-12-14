using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Categories_CategoryId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Statuses_StatusId",
                table: "Tasks");

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Projects",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                column: "Budget",
                value: null);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                column: "Budget",
                value: null);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                column: "Budget",
                value: null);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                column: "Budget",
                value: null);

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

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { 1, 2 });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { 1, 4 });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { 1, 2 });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { 1, 3 });

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CategoryId", "StatusId" },
                values: new object[] { 4, 1 });

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Categories_CategoryId",
                table: "Tasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Statuses_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
