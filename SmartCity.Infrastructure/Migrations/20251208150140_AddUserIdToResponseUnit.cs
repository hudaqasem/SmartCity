using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToResponseUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ResponseUnits",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResponseUnits_UserId",
                table: "ResponseUnits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseUnits_AspNetUsers_UserId",
                table: "ResponseUnits",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseUnits_AspNetUsers_UserId",
                table: "ResponseUnits");

            migrationBuilder.DropIndex(
                name: "IX_ResponseUnits_UserId",
                table: "ResponseUnits");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ResponseUnits");
        }
    }
}
