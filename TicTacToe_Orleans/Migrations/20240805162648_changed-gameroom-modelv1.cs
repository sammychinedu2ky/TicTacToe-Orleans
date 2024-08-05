using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe_Orleans_.Migrations
{
    /// <inheritdoc />
    public partial class changedgameroommodelv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moves",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "Winner",
                table: "GameRooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Moves",
                table: "GameRooms",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Winner",
                table: "GameRooms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
