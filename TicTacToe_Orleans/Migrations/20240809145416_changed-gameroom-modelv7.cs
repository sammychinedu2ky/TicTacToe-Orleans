using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe_Orleans_.Migrations
{
    /// <inheritdoc />
    public partial class changedgameroommodelv7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Invites",
                table: "Invites");

            migrationBuilder.RenameTable(
                name: "Invites",
                newName: "Invitations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invitations",
                table: "Invitations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Invitations",
                table: "Invitations");

            migrationBuilder.RenameTable(
                name: "Invitations",
                newName: "Invites");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invites",
                table: "Invites",
                column: "Id");
        }
    }
}
