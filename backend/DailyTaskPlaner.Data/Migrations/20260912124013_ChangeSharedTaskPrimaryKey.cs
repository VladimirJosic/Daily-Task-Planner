using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyTaskPlaner.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSharedTaskPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedTasks",
                table: "SharedTasks");

            migrationBuilder.DropIndex(
                name: "IX_SharedTasks_DailyTaskId",
                table: "SharedTasks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedTasks",
                table: "SharedTasks",
                columns: new[] { "DailyTaskId", "FriendId" });

            migrationBuilder.CreateIndex(
                name: "IX_SharedTasks_UserId",
                table: "SharedTasks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedTasks",
                table: "SharedTasks");

            migrationBuilder.DropIndex(
                name: "IX_SharedTasks_UserId",
                table: "SharedTasks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedTasks",
                table: "SharedTasks",
                columns: new[] { "UserId", "DailyTaskId" });

            migrationBuilder.CreateIndex(
                name: "IX_SharedTasks_DailyTaskId",
                table: "SharedTasks",
                column: "DailyTaskId");
        }
    }
}
