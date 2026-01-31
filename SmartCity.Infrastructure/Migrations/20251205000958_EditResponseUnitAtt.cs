using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditResponseUnitAtt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "ResponseUnits");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "ResponseUnits",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "ResponseUnits",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "ResponseUnits");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "ResponseUnits");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ResponseUnits",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
