using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyTaskPlaner.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenAndTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First clean up existing data
            migrationBuilder.Sql(@"
                -- Set empty usernames to a unique value based on ID
                UPDATE Users SET Username = 'user_' + CAST(Id AS VARCHAR) 
                WHERE Username IS NULL OR Username = ''
        
                -- Handle any remaining duplicates
                ;WITH Duplicates AS (
                    SELECT 
                        Username, 
                        ROW_NUMBER() OVER (PARTITION BY Username ORDER BY Id) AS RowNum
                    FROM Users
                )
                UPDATE u SET u.Username = u.Username + '_' + CAST(d.RowNum AS VARCHAR)
                FROM Users u
                JOIN Duplicates d ON u.Username = d.Username AND d.RowNum > 1
            ");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "DailyTasks",
                newName: "IsUrgent");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "DailyTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "DailyTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "DailyTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SharedTasks",
                columns: table => new
                {
                    DailyTaskId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    FriendId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedTasks", x => new { x.UserId, x.DailyTaskId });
                    table.ForeignKey(
                        name: "FK_SharedTasks_DailyTasks_DailyTaskId",
                        column: x => x.DailyTaskId,
                        principalTable: "DailyTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedTasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyTasks_UserId",
                table: "DailyTasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedTasks_DailyTaskId",
                table: "SharedTasks",
                column: "DailyTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyTasks_Users_UserId",
                table: "DailyTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyTasks_Users_UserId",
                table: "DailyTasks");

            migrationBuilder.DropTable(
                name: "SharedTasks");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_DailyTasks_UserId",
                table: "DailyTasks");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "DailyTasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DailyTasks");

            migrationBuilder.RenameColumn(
                name: "IsUrgent",
                table: "DailyTasks",
                newName: "Priority");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "DailyTasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
