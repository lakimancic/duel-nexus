using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class GameCardFkFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameCards_Cards_CardId",
                table: "GameCards");

            migrationBuilder.DropIndex(
                name: "IX_GameCards_PlayerGameId_CardId",
                table: "GameCards");

            migrationBuilder.RenameColumn(
                name: "CardId",
                table: "GameCards",
                newName: "PlayerCardId");

            migrationBuilder.RenameIndex(
                name: "IX_GameCards_CardId",
                table: "GameCards",
                newName: "IX_GameCards_PlayerCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCards_PlayerGameId",
                table: "GameCards",
                column: "PlayerGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameCards_PlayerCards_PlayerCardId",
                table: "GameCards",
                column: "PlayerCardId",
                principalTable: "PlayerCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameCards_PlayerCards_PlayerCardId",
                table: "GameCards");

            migrationBuilder.DropIndex(
                name: "IX_GameCards_PlayerGameId",
                table: "GameCards");

            migrationBuilder.RenameColumn(
                name: "PlayerCardId",
                table: "GameCards",
                newName: "CardId");

            migrationBuilder.RenameIndex(
                name: "IX_GameCards_PlayerCardId",
                table: "GameCards",
                newName: "IX_GameCards_CardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCards_PlayerGameId_CardId",
                table: "GameCards",
                columns: new[] { "PlayerGameId", "CardId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameCards_Cards_CardId",
                table: "GameCards",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
