using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDirectGameRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EffectActivations_Games_GameId",
                table: "EffectActivations");

            migrationBuilder.DropIndex(
                name: "IX_EffectActivations_GameId",
                table: "EffectActivations");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "EffectActivations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                table: "EffectActivations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EffectActivations_GameId",
                table: "EffectActivations",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_EffectActivations_Games_GameId",
                table: "EffectActivations",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
