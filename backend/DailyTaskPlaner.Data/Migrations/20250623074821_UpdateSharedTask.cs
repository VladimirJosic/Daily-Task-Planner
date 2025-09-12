using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyTaskPlaner.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSharedTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "SharedTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SharedTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
