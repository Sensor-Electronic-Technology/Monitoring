using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class ChannelMapping_Actions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_ActionRegisterType",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_ActionStart",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_ActionStop",
                table: "Devices",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelMapping_ActionRegisterType",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_ActionStart",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_ActionStop",
                table: "Devices");
        }
    }
}
