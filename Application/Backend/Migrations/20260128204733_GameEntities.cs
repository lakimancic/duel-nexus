using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class GameEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Decks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Effects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Affects = table.Column<int>(type: "integer", nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: true),
                    Turns = table.Column<int>(type: "integer", nullable: true),
                    RequiresTarget = table.Column<bool>(type: "boolean", nullable: false),
                    TargetsPlayer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Effects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRanked = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    JoinCode = table.Column<string>(type: "text", nullable: true),
                    HostUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRooms_Users_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EffectId = table.Column<Guid>(type: "uuid", nullable: true),
                    Attack = table.Column<int>(type: "integer", nullable: true),
                    Defense = table.Column<int>(type: "integer", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Effects_EffectId",
                        column: x => x.EffectId,
                        principalTable: "Effects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameRoomPlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsReady = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRoomPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRoomPlayers_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameRoomPlayers_GameRooms_GameRoomId",
                        column: x => x.GameRoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameRoomPlayers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_GameRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeckCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeckCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeckCards_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    LifePoints = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerGames_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerGames_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerGames_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerGameId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Zone = table.Column<int>(type: "integer", nullable: false),
                    DeckOrder = table.Column<int>(type: "integer", nullable: true),
                    IsFaceDown = table.Column<bool>(type: "boolean", nullable: false),
                    FieldIndex = table.Column<int>(type: "integer", nullable: true),
                    DefensePosition = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameCards_PlayerGames_PlayerGameId",
                        column: x => x.PlayerGameId,
                        principalTable: "PlayerGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Turns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnNumber = table.Column<int>(type: "integer", nullable: false),
                    ActivePlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Phase = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turns_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Turns_PlayerGames_ActivePlayerId",
                        column: x => x.ActivePlayerId,
                        principalTable: "PlayerGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttackActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttackerCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    DefenderCardId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttackActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttackActions_GameCards_AttackerCardId",
                        column: x => x.AttackerCardId,
                        principalTable: "GameCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttackActions_GameCards_DefenderCardId",
                        column: x => x.DefenderCardId,
                        principalTable: "GameCards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttackActions_Turns_TurnId",
                        column: x => x.TurnId,
                        principalTable: "Turns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardMovementActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerGameId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromZone = table.Column<int>(type: "integer", nullable: false),
                    ToZone = table.Column<int>(type: "integer", nullable: false),
                    MovementType = table.Column<int>(type: "integer", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardMovementActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardMovementActions_GameCards_GameCardId",
                        column: x => x.GameCardId,
                        principalTable: "GameCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardMovementActions_PlayerGames_PlayerGameId",
                        column: x => x.PlayerGameId,
                        principalTable: "PlayerGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardMovementActions_Turns_TurnId",
                        column: x => x.TurnId,
                        principalTable: "Turns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EffectActivations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerGameId = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Resolved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectActivations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EffectActivations_Effects_EffectId",
                        column: x => x.EffectId,
                        principalTable: "Effects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EffectActivations_GameCards_SourceCardId",
                        column: x => x.SourceCardId,
                        principalTable: "GameCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EffectActivations_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EffectActivations_PlayerGames_PlayerGameId",
                        column: x => x.PlayerGameId,
                        principalTable: "PlayerGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EffectActivations_Turns_TurnId",
                        column: x => x.TurnId,
                        principalTable: "Turns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceCardActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerGameId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldIndex = table.Column<int>(type: "integer", nullable: true),
                    FaceDown = table.Column<bool>(type: "boolean", nullable: false),
                    DefensePosition = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceCardActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceCardActions_GameCards_GameCardId",
                        column: x => x.GameCardId,
                        principalTable: "GameCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceCardActions_PlayerGames_PlayerGameId",
                        column: x => x.PlayerGameId,
                        principalTable: "PlayerGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceCardActions_Turns_TurnId",
                        column: x => x.TurnId,
                        principalTable: "Turns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EffectResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AffectedCardId = table.Column<Guid>(type: "uuid", nullable: true),
                    AffectedPlayerId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreviousZone = table.Column<int>(type: "integer", nullable: true),
                    NewZone = table.Column<int>(type: "integer", nullable: true),
                    LifePointsChange = table.Column<int>(type: "integer", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "EffectTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetCardId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetPlayerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EffectTargets_EffectActivations_ActivationId",
                        column: x => x.ActivationId,
                        principalTable: "EffectActivations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EffectTargets_GameCards_TargetCardId",
                        column: x => x.TargetCardId,
                        principalTable: "GameCards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EffectTargets_PlayerGames_TargetPlayerId",
                        column: x => x.TargetPlayerId,
                        principalTable: "PlayerGames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttackActions_AttackerCardId",
                table: "AttackActions",
                column: "AttackerCardId");

            migrationBuilder.CreateIndex(
                name: "IX_AttackActions_DefenderCardId",
                table: "AttackActions",
                column: "DefenderCardId");

            migrationBuilder.CreateIndex(
                name: "IX_AttackActions_TurnId",
                table: "AttackActions",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_CardMovementActions_GameCardId",
                table: "CardMovementActions",
                column: "GameCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardMovementActions_PlayerGameId",
                table: "CardMovementActions",
                column: "PlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_CardMovementActions_TurnId",
                table: "CardMovementActions",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_EffectId",
                table: "Cards",
                column: "EffectId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_CardId",
                table: "DeckCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_DeckId",
                table: "DeckCards",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_UserId",
                table: "Decks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_EffectId",
                table: "EffectActivations",
                column: "EffectId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_GameId",
                table: "EffectActivations",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_PlayerGameId",
                table: "EffectActivations",
                column: "PlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_SourceCardId",
                table: "EffectActivations",
                column: "SourceCardId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_TurnId",
                table: "EffectActivations",
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

            migrationBuilder.CreateIndex(
                name: "IX_EffectTargets_ActivationId",
                table: "EffectTargets",
                column: "ActivationId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectTargets_TargetCardId",
                table: "EffectTargets",
                column: "TargetCardId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectTargets_TargetPlayerId",
                table: "EffectTargets",
                column: "TargetPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCards_CardId",
                table: "GameCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCards_PlayerGameId",
                table: "GameCards",
                column: "PlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRoomPlayers_DeckId",
                table: "GameRoomPlayers",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRoomPlayers_GameRoomId",
                table: "GameRoomPlayers",
                column: "GameRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRoomPlayers_UserId",
                table: "GameRoomPlayers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRooms_HostUserId",
                table: "GameRooms",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_RoomId",
                table: "Games",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceCardActions_GameCardId",
                table: "PlaceCardActions",
                column: "GameCardId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceCardActions_PlayerGameId",
                table: "PlaceCardActions",
                column: "PlayerGameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceCardActions_TurnId",
                table: "PlaceCardActions",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGames_DeckId",
                table: "PlayerGames",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGames_GameId",
                table: "PlayerGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGames_UserId",
                table: "PlayerGames",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Turns_ActivePlayerId",
                table: "Turns",
                column: "ActivePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Turns_GameId",
                table: "Turns",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttackActions");

            migrationBuilder.DropTable(
                name: "CardMovementActions");

            migrationBuilder.DropTable(
                name: "DeckCards");

            migrationBuilder.DropTable(
                name: "EffectResults");

            migrationBuilder.DropTable(
                name: "EffectTargets");

            migrationBuilder.DropTable(
                name: "GameRoomPlayers");

            migrationBuilder.DropTable(
                name: "PlaceCardActions");

            migrationBuilder.DropTable(
                name: "EffectActivations");

            migrationBuilder.DropTable(
                name: "GameCards");

            migrationBuilder.DropTable(
                name: "Turns");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "PlayerGames");

            migrationBuilder.DropTable(
                name: "Effects");

            migrationBuilder.DropTable(
                name: "Decks");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "GameRooms");
        }
    }
}
