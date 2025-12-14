using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaskDetailsToDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Tasks",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Tasks",
                newName: "Details");
        }
    }
}
