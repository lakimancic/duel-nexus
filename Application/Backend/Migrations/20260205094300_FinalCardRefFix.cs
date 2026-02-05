using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FinalCardRefFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_PlayerCards_CardId",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCards_Decks_DeckId",
                table: "PlayerCards");

            migrationBuilder.RenameColumn(
                name: "DeckId",
                table: "PlayerCards",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerCards_DeckId_CardId",
                table: "PlayerCards",
                newName: "IX_PlayerCards_UserId_CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_Cards_CardId",
                table: "DeckCards",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCards_Users_UserId",
                table: "PlayerCards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_Cards_CardId",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCards_Users_UserId",
                table: "PlayerCards");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PlayerCards",
                newName: "DeckId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerCards_UserId_CardId",
                table: "PlayerCards",
                newName: "IX_PlayerCards_DeckId_CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_PlayerCards_CardId",
                table: "DeckCards",
                column: "CardId",
                principalTable: "PlayerCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCards_Decks_DeckId",
                table: "PlayerCards",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
