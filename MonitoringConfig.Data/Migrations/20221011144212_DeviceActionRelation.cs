using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class DeviceActionRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceActions_Devices_MonitorBoxId",
                table: "DeviceActions");

            migrationBuilder.RenameColumn(
                name: "MonitorBoxId",
                table: "DeviceActions",
                newName: "ModbusDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceActions_MonitorBoxId",
                table: "DeviceActions",
                newName: "IX_DeviceActions_ModbusDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceActions_Devices_ModbusDeviceId",
                table: "DeviceActions",
                column: "ModbusDeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceActions_Devices_ModbusDeviceId",
                table: "DeviceActions");

            migrationBuilder.RenameColumn(
                name: "ModbusDeviceId",
                table: "DeviceActions",
                newName: "MonitorBoxId");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceActions_ModbusDeviceId",
                table: "DeviceActions",
                newName: "IX_DeviceActions_MonitorBoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceActions_Devices_MonitorBoxId",
                table: "DeviceActions",
                column: "MonitorBoxId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
