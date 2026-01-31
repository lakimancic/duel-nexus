using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class NullabilityFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EffectTargets_PlayerGames_TargetPlayerId",
                table: "EffectTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_GameRoomPlayers_Decks_DeckId",
                table: "GameRoomPlayers");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeckId",
                table: "GameRoomPlayers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetPlayerId",
                table: "EffectTargets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectTargets_PlayerGames_TargetPlayerId",
                table: "EffectTargets",
                column: "TargetPlayerId",
                principalTable: "PlayerGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameRoomPlayers_Decks_DeckId",
                table: "GameRoomPlayers",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EffectTargets_PlayerGames_TargetPlayerId",
                table: "EffectTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_GameRoomPlayers_Decks_DeckId",
                table: "GameRoomPlayers");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeckId",
                table: "GameRoomPlayers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetPlayerId",
                table: "EffectTargets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_EffectTargets_PlayerGames_TargetPlayerId",
                table: "EffectTargets",
                column: "TargetPlayerId",
                principalTable: "PlayerGames",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameRoomPlayers_Decks_DeckId",
                table: "GameRoomPlayers",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
