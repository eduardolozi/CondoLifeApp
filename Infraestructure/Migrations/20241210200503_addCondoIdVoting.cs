using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class addCondoIdVoting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CondominiumId",
                table: "Voting",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Voting_CondominiumId",
                table: "Voting",
                column: "CondominiumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voting_Condominium_CondominiumId",
                table: "Voting",
                column: "CondominiumId",
                principalTable: "Condominium",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voting_Condominium_CondominiumId",
                table: "Voting");

            migrationBuilder.DropIndex(
                name: "IX_Voting_CondominiumId",
                table: "Voting");

            migrationBuilder.DropColumn(
                name: "CondominiumId",
                table: "Voting");
        }
    }
}
