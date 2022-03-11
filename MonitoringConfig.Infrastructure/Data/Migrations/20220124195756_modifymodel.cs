using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class modifymodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxModules_ModbusDevices_MonitoringBoxesId",
                table: "BoxModules");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_ModbusDevices_ModbusDeviceId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceZones_ModbusDevices_ModbusDevicesId",
                table: "DeviceZones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModbusDevices",
                table: "ModbusDevices");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ModbusDevices");

            migrationBuilder.RenameTable(
                name: "ModbusDevices",
                newName: "Device");

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

            migrationBuilder.AddColumn<bool>(
                name: "Bypass",
                table: "Channels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Display",
                table: "Channels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ApiToken",
                table: "Device",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_AlertRegisterType",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_AlertStart",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_AlertStop",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_AnalogRegisterType",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_AnalogStart",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_AnalogStop",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_DeviceRegisterType",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_DeviceStart",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_DeviceStop",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_DiscreteRegisterType",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_DiscreteStart",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_DiscreteStop",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_VirtualRegisterType",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_VirtualStart",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMapping_VirtualStop",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModbusAddress_Address",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModbusAddress_RegisterLength",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModbusAddress_RegisterType",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NetworkConfiguration_ModbusConfig_HoldingRegisters",
                table: "Device",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Bypass",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ApiToken",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_AlertRegisterType",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_AlertStart",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_AlertStop",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_AnalogRegisterType",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_AnalogStart",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_AnalogStop",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_DeviceRegisterType",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_DeviceStart",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_DeviceStop",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_DiscreteRegisterType",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_DiscreteStart",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_DiscreteStop",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_VirtualRegisterType",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_VirtualStart",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ChannelMapping_VirtualStop",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ModbusAddress_Address",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ModbusAddress_RegisterLength",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ModbusAddress_RegisterType",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "NetworkConfiguration_ModbusConfig_HoldingRegisters",
                table: "Device");

            migrationBuilder.RenameTable(
                name: "Device",
                newName: "ModbusDevices");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "ModbusDevices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModbusDevices",
                table: "ModbusDevices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxModules_ModbusDevices_MonitoringBoxesId",
                table: "BoxModules",
                column: "MonitoringBoxesId",
                principalTable: "ModbusDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_ModbusDevices_ModbusDeviceId",
                table: "Channels",
                column: "ModbusDeviceId",
                principalTable: "ModbusDevices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceZones_ModbusDevices_ModbusDevicesId",
                table: "DeviceZones",
                column: "ModbusDevicesId",
                principalTable: "ModbusDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
