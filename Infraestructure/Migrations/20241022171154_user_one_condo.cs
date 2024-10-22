using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class user_one_condo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CondominiumUser");

            migrationBuilder.AddColumn<int>(
                name: "CondominiumId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CondominiumId",
                table: "Users",
                column: "CondominiumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Condominium_CondominiumId",
                table: "Users",
                column: "CondominiumId",
                principalTable: "Condominium",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Condominium_CondominiumId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CondominiumId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CondominiumId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "CondominiumUser",
                columns: table => new
                {
                    CondominiumId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondominiumUser", x => new { x.CondominiumId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CondominiumUser_Condominium_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominium",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CondominiumUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CondominiumUser_UsersId",
                table: "CondominiumUser",
                column: "UsersId");
        }
    }
}
