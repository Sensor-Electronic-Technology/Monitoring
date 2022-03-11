using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class DbsetName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxModules_Device_MonitoringBoxesId",
                table: "BoxModules");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Device_ModbusDeviceId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceZones_Device_ModbusDevicesId",
                table: "DeviceZones");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityZones_Device_ApiDeviceId",
                table: "FacilityZones");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityZones_Device_BnetDeviceId",
                table: "FacilityZones");

            migrationBuilder.DropIndex(
                name: "IX_FacilityZones_ApiDeviceId",
                table: "FacilityZones");

            migrationBuilder.DropIndex(
                name: "IX_FacilityZones_BnetDeviceId",
                table: "FacilityZones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Device",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ApiDeviceId",
                table: "FacilityZones");

            migrationBuilder.DropColumn(
                name: "BnetDeviceId",
                table: "FacilityZones");

            migrationBuilder.RenameTable(
                name: "Device",
                newName: "Devices");

            migrationBuilder.RenameColumn(
                name: "ModbusDevicesId",
                table: "DeviceZones",
                newName: "DevicesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Devices",
                table: "Devices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxModules_Devices_MonitoringBoxesId",
                table: "BoxModules",
                column: "MonitoringBoxesId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Devices_ModbusDeviceId",
                table: "Channels",
                column: "ModbusDeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceZones_Devices_DevicesId",
                table: "DeviceZones",
                column: "DevicesId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxModules_Devices_MonitoringBoxesId",
                table: "BoxModules");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Devices_ModbusDeviceId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceZones_Devices_DevicesId",
                table: "DeviceZones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Devices",
                table: "Devices");

            migrationBuilder.RenameTable(
                name: "Devices",
                newName: "Device");

            migrationBuilder.RenameColumn(
                name: "DevicesId",
                table: "DeviceZones",
                newName: "ModbusDevicesId");

            migrationBuilder.AddColumn<int>(
                name: "ApiDeviceId",
                table: "FacilityZones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BnetDeviceId",
                table: "FacilityZones",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Device",
                table: "Device",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityZones_ApiDeviceId",
                table: "FacilityZones",
                column: "ApiDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityZones_BnetDeviceId",
                table: "FacilityZones",
                column: "BnetDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxModules_Device_MonitoringBoxesId",
                table: "BoxModules",
                column: "MonitoringBoxesId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Device_ModbusDeviceId",
                table: "Channels",
                column: "ModbusDeviceId",
                principalTable: "Device",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceZones_Device_ModbusDevicesId",
                table: "DeviceZones",
                column: "ModbusDevicesId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityZones_Device_ApiDeviceId",
                table: "FacilityZones",
                column: "ApiDeviceId",
                principalTable: "Device",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityZones_Device_BnetDeviceId",
                table: "FacilityZones",
                column: "BnetDeviceId",
                principalTable: "Device",
                principalColumn: "Id");
        }
    }
}
