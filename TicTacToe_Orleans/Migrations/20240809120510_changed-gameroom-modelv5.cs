using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe_Orleans_.Migrations
{
    /// <inheritdoc />
    public partial class changedgameroommodelv5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Board",
                table: "GameRooms",
                type: "json",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Board",
                table: "GameRooms",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "json");
        }
    }
}
