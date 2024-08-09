using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe_Orleans_.Migrations
{
    /// <inheritdoc />
    public partial class changedgameroommodelv6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Board",
                table: "GameRooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Board",
                table: "GameRooms",
                type: "json",
                nullable: false,
                defaultValue: "");
        }
    }
}
