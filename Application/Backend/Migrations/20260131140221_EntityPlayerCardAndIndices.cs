using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class EntityPlayerCardAndIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardMovementActions_PlayerGames_PlayerGameId",
                table: "CardMovementActions");

            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_Cards_CardId",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectActivations_PlayerGames_PlayerGameId",
                table: "EffectActivations");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaceCardActions_PlayerGames_PlayerGameId",
                table: "PlaceCardActions");

            migrationBuilder.DropTable(
                name: "EffectResults");

            migrationBuilder.DropIndex(
                name: "IX_Turns_GameId",
                table: "Turns");

            migrationBuilder.DropIndex(
                name: "IX_PlayerGames_GameId",
                table: "PlayerGames");

            migrationBuilder.DropIndex(
                name: "IX_PlaceCardActions_PlayerGameId",
                table: "PlaceCardActions");

            migrationBuilder.DropIndex(
                name: "IX_PlaceCardActions_TurnId",
                table: "PlaceCardActions");

            migrationBuilder.DropIndex(
                name: "IX_Games_RoomId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_GameCards_PlayerGameId",
                table: "GameCards");

            migrationBuilder.DropIndex(
                name: "IX_EffectTargets_ActivationId",
                table: "EffectTargets");

            migrationBuilder.DropIndex(
                name: "IX_EffectActivations_PlayerGameId",
                table: "EffectActivations");

            migrationBuilder.DropIndex(
                name: "IX_EffectActivations_TurnId",
                table: "EffectActivations");

            migrationBuilder.DropIndex(
                name: "IX_Decks_UserId",
                table: "Decks");

            migrationBuilder.DropIndex(
                name: "IX_DeckCards_DeckId",
                table: "DeckCards");

            migrationBuilder.DropIndex(
                name: "IX_CardMovementActions_PlayerGameId",
                table: "CardMovementActions");

            migrationBuilder.DropIndex(
                name: "IX_CardMovementActions_TurnId",
                table: "CardMovementActions");

            migrationBuilder.DropIndex(
                name: "IX_AttackActions_TurnId",
                table: "AttackActions");

            migrationBuilder.DropColumn(
                name: "PlayerGameId",
                table: "PlaceCardActions");

            migrationBuilder.DropColumn(
                name: "PlayerGameId",
                table: "EffectActivations");

            migrationBuilder.DropColumn(
                name: "PlayerGameId",
                table: "CardMovementActions");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "GameRooms",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "Decks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DefenderPlayerGameId",
                table: "AttackActions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlayerCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCards_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCards_PlayerCards_CardId",
                        column: x => x.CardId,
                        principalTable: "PlayerCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turns_GameId_TurnNumber",
                table: "Turns",
                columns: new[] { "GameId", "TurnNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGames_GameId_UserId",
                table: "PlayerGames",
                columns: new[] { "GameId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceCardActions_TurnId_GameCardId",
                table: "PlaceCardActions",
                columns: new[] { "TurnId", "GameCardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_RoomId",
                table: "Games",
                column: "RoomId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameCards_PlayerGameId_CardId",
                table: "GameCards",
                columns: new[] { "PlayerGameId", "CardId" });

            migrationBuilder.CreateIndex(
                name: "IX_EffectTargets_ActivationId_TargetCardId",
                table: "EffectTargets",
                columns: new[] { "ActivationId", "TargetCardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_TurnId_SourceCardId",
                table: "EffectActivations",
                columns: new[] { "TurnId", "SourceCardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decks_UserId_Name",
                table: "Decks",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_DeckId_CardId",
                table: "DeckCards",
                columns: new[] { "DeckId", "CardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardMovementActions_TurnId_GameCardId",
                table: "CardMovementActions",
                columns: new[] { "TurnId", "GameCardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttackActions_DefenderPlayerGameId",
                table: "AttackActions",
                column: "DefenderPlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_AttackActions_TurnId_AttackerCardId",
                table: "AttackActions",
                columns: new[] { "TurnId", "AttackerCardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCards_CardId",
                table: "PlayerCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCards_DeckId_CardId",
                table: "PlayerCards",
                columns: new[] { "DeckId", "CardId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttackActions_PlayerGames_DefenderPlayerGameId",
                table: "AttackActions",
                column: "DefenderPlayerGameId",
                principalTable: "PlayerGames",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_PlayerCards_CardId",
                table: "DeckCards",
                column: "CardId",
                principalTable: "PlayerCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttackActions_PlayerGames_DefenderPlayerGameId",
                table: "AttackActions");

            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_PlayerCards_CardId",
                table: "DeckCards");

            migrationBuilder.DropTable(
                name: "PlayerCards");

            migrationBuilder.DropIndex(
                name: "IX_Turns_GameId_TurnNumber",
                table: "Turns");

            migrationBuilder.DropIndex(
                name: "IX_PlayerGames_GameId_UserId",
                table: "PlayerGames");

            migrationBuilder.DropIndex(
                name: "IX_PlaceCardActions_TurnId_GameCardId",
                table: "PlaceCardActions");

            migrationBuilder.DropIndex(
                name: "IX_Games_RoomId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_GameCards_PlayerGameId_CardId",
                table: "GameCards");

            migrationBuilder.DropIndex(
                name: "IX_EffectTargets_ActivationId_TargetCardId",
                table: "EffectTargets");

            migrationBuilder.DropIndex(
                name: "IX_EffectActivations_TurnId_SourceCardId",
                table: "EffectActivations");

            migrationBuilder.DropIndex(
                name: "IX_Decks_UserId_Name",
                table: "Decks");

            migrationBuilder.DropIndex(
                name: "IX_DeckCards_DeckId_CardId",
                table: "DeckCards");

            migrationBuilder.DropIndex(
                name: "IX_CardMovementActions_TurnId_GameCardId",
                table: "CardMovementActions");

            migrationBuilder.DropIndex(
                name: "IX_AttackActions_DefenderPlayerGameId",
                table: "AttackActions");

            migrationBuilder.DropIndex(
                name: "IX_AttackActions_TurnId_AttackerCardId",
                table: "AttackActions");

            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "Decks");

            migrationBuilder.DropColumn(
                name: "DefenderPlayerGameId",
                table: "AttackActions");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "GameRooms",
                newName: "CreatedAtUtc");

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerGameId",
                table: "PlaceCardActions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerGameId",
                table: "EffectActivations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerGameId",
                table: "CardMovementActions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "EffectResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AffectedCardId = table.Column<Guid>(type: "uuid", nullable: true),
                    AffectedPlayerId = table.Column<Guid>(type: "uuid", nullable: true),
                    LifePointsChange = table.Column<int>(type: "integer", nullable: true),
                    NewZone = table.Column<int>(type: "integer", nullable: true),
                    PreviousZone = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EffectResults_EffectActivations_ActivationId",
                        column: x => x.ActivationId,
                        principalTable: "EffectActivations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EffectResults_GameCards_AffectedCardId",
                        column: x => x.AffectedCardId,
                        principalTable: "GameCards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EffectResults_PlayerGames_AffectedPlayerId",
                        column: x => x.AffectedPlayerId,
                        principalTable: "PlayerGames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turns_GameId",
                table: "Turns",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGames_GameId",
                table: "PlayerGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceCardActions_PlayerGameId",
                table: "PlaceCardActions",
                column: "PlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceCardActions_TurnId",
                table: "PlaceCardActions",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_RoomId",
                table: "Games",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCards_PlayerGameId",
                table: "GameCards",
                column: "PlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectTargets_ActivationId",
                table: "EffectTargets",
                column: "ActivationId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_PlayerGameId",
                table: "EffectActivations",
                column: "PlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_TurnId",
                table: "EffectActivations",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_UserId",
                table: "Decks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_DeckId",
                table: "DeckCards",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_CardMovementActions_PlayerGameId",
                table: "CardMovementActions",
                column: "PlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_CardMovementActions_TurnId",
                table: "CardMovementActions",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_AttackActions_TurnId",
                table: "AttackActions",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectResults_ActivationId",
                table: "EffectResults",
                column: "ActivationId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectResults_AffectedCardId",
                table: "EffectResults",
                column: "AffectedCardId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectResults_AffectedPlayerId",
                table: "EffectResults",
                column: "AffectedPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardMovementActions_PlayerGames_PlayerGameId",
                table: "CardMovementActions",
                column: "PlayerGameId",
                principalTable: "PlayerGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_Cards_CardId",
                table: "DeckCards",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectActivations_PlayerGames_PlayerGameId",
                table: "EffectActivations",
                column: "PlayerGameId",
                principalTable: "PlayerGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaceCardActions_PlayerGames_PlayerGameId",
                table: "PlaceCardActions",
                column: "PlayerGameId",
                principalTable: "PlayerGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
