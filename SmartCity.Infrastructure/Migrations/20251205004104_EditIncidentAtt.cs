using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditIncidentAtt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Incidents");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Incidents",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Incidents",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Incidents");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
