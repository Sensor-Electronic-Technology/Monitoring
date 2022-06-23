using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class SensorPlotInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YAxisMin",
                table: "Sensors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YAxitMax",
                table: "Sensors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YAxisMin",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "YAxitMax",
                table: "Sensors");
        }
    }
}
