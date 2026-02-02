using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class PlayerCardCircleFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameCards_PlayerCards_PlayerCardId",
                table: "GameCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCards_PlayerCards_CardId",
                table: "PlayerCards");

            migrationBuilder.DropIndex(
                name: "IX_GameCards_PlayerCardId",
                table: "GameCards");

            migrationBuilder.RenameColumn(
                name: "PlayerCardId",
                table: "GameCards",
                newName: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCards_CardId_PlayerGameId",
                table: "GameCards",
                columns: new[] { "CardId", "PlayerGameId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameCards_Cards_CardId",
                table: "GameCards",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCards_Cards_CardId",
                table: "PlayerCards",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameCards_Cards_CardId",
                table: "GameCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCards_Cards_CardId",
                table: "PlayerCards");

            migrationBuilder.DropIndex(
                name: "IX_GameCards_CardId_PlayerGameId",
                table: "GameCards");

            migrationBuilder.RenameColumn(
                name: "CardId",
                table: "GameCards",
                newName: "PlayerCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCards_PlayerCardId",
                table: "GameCards",
                column: "PlayerCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameCards_PlayerCards_PlayerCardId",
                table: "GameCards",
                column: "PlayerCardId",
                principalTable: "PlayerCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCards_PlayerCards_CardId",
                table: "PlayerCards",
                column: "CardId",
                principalTable: "PlayerCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
