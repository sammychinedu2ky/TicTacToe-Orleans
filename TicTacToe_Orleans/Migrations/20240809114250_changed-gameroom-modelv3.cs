using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe_Orleans_.Migrations
{
    /// <inheritdoc />
    public partial class changedgameroommodelv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<List<char>>>(
                name: "Board",
                table: "GameRooms",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(List<List<char>>),
                oldType: "json");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<List<char>>>(
                name: "Board",
                table: "GameRooms",
                type: "json",
                nullable: false,
                oldClrType: typeof(List<List<char>>),
                oldType: "jsonb");
        }
    }
}
