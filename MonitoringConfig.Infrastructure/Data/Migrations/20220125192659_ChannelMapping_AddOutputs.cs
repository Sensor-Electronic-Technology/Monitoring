using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class ChannelMapping_AddOutputs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_OutputRegisterType",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_OutputStart",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_OutputStop",
                table: "Devices",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelMapping_OutputRegisterType",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_OutputStart",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_OutputStop",
                table: "Devices");
        }
    }
}
