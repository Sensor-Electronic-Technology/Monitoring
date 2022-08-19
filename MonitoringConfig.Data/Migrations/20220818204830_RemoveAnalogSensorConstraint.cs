using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class RemoveAnalogSensorConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Sensors_SensorId",
                table: "Channels");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Sensors_SensorId",
                table: "Channels",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Sensors_SensorId",
                table: "Channels");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Sensors_SensorId",
                table: "Channels",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
