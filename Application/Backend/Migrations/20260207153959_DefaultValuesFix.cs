using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class DefaultValuesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerGames_Decks_DeckId",
                table: "PlayerGames");

            migrationBuilder.DropForeignKey(
                name: "FK_Turns_PlayerGames_ActivePlayerId",
                table: "Turns");

            migrationBuilder.DropIndex(
                name: "IX_PlayerGames_DeckId",
                table: "PlayerGames");

            migrationBuilder.DropColumn(
                name: "DeckId",
                table: "PlayerGames");

            migrationBuilder.AlterColumn<Guid>(
                name: "ActivePlayerId",
                table: "Turns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Turns_PlayerGames_ActivePlayerId",
                table: "Turns",
                column: "ActivePlayerId",
                principalTable: "PlayerGames",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turns_PlayerGames_ActivePlayerId",
                table: "Turns");

            migrationBuilder.AlterColumn<Guid>(
                name: "ActivePlayerId",
                table: "Turns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeckId",
                table: "PlayerGames",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGames_DeckId",
                table: "PlayerGames",
                column: "DeckId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerGames_Decks_DeckId",
                table: "PlayerGames",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turns_PlayerGames_ActivePlayerId",
                table: "Turns",
                column: "ActivePlayerId",
                principalTable: "PlayerGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
