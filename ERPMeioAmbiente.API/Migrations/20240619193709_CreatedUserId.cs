using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPMeioAmbiente.API.Migrations
{
    /// <inheritdoc />
    public partial class CreatedUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Clientes",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UserId",
                table: "Clientes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_AspNetUsers_UserId",
                table: "Clientes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_AspNetUsers_UserId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UserId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Clientes");
        }
    }
}
