using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlaner.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUser_AppUser_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AppUserId",
                table: "Users",
                column: "AppUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AppUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Users");
        }
    }
}
