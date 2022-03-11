using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class MovedChannelMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModbusAddress_RegisterType",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ModbusAddress_RegisterType");

            migrationBuilder.RenameColumn(
                name: "ModbusAddress_RegisterLength",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ModbusAddress_RegisterLength");

            migrationBuilder.RenameColumn(
                name: "ModbusAddress_Address",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ModbusAddress_Address");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_VirtualStop",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_VirtualStop");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_VirtualStart",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_VirtualStart");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_VirtualRegisterType",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_VirtualRegisterType");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_OutputStop",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_OutputStop");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_OutputStart",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_OutputStart");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_OutputRegisterType",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_OutputRegisterType");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_DiscreteStop",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_DiscreteStop");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_DiscreteStart",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_DiscreteStart");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_DiscreteRegisterType",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_DiscreteRegisterType");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_DeviceStop",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_DeviceStop");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_DeviceStart",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_DeviceStart");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_DeviceRegisterType",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_DeviceRegisterType");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_AnalogStop",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_AnalogStop");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_AnalogStart",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_AnalogStart");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_AnalogRegisterType",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_AnalogRegisterType");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_AlertStop",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_AlertStop");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_AlertStart",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_AlertStart");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_AlertRegisterType",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_AlertRegisterType");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_ActionStop",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_ActionStop");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_ActionStart",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_ActionStart");

            migrationBuilder.RenameColumn(
                name: "ChannelMapping_ActionRegisterType",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_ChannelMapping_ActionRegisterType");

            migrationBuilder.RenameColumn(
                name: "MobdusAddress_RegisterType",
                table: "Alerts",
                newName: "ModbusAddress_RegisterType");

            migrationBuilder.RenameColumn(
                name: "MobdusAddress_RegisterLength",
                table: "Alerts",
                newName: "ModbusAddress_RegisterLength");

            migrationBuilder.RenameColumn(
                name: "MobdusAddress_Address",
                table: "Alerts",
                newName: "ModbusAddress_Address");

            migrationBuilder.AddColumn<bool>(
                name: "Bypass",
                table: "AlertLevel",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BypassResetTime",
                table: "AlertLevel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "AlertLevel",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bypass",
                table: "AlertLevel");

            migrationBuilder.DropColumn(
                name: "BypassResetTime",
                table: "AlertLevel");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "AlertLevel");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ModbusAddress_RegisterType",
                table: "Devices",
                newName: "ModbusAddress_RegisterType");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ModbusAddress_RegisterLength",
                table: "Devices",
                newName: "ModbusAddress_RegisterLength");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ModbusAddress_Address",
                table: "Devices",
                newName: "ModbusAddress_Address");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_VirtualStop",
                table: "Devices",
                newName: "ChannelMapping_VirtualStop");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_VirtualStart",
                table: "Devices",
                newName: "ChannelMapping_VirtualStart");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_VirtualRegisterType",
                table: "Devices",
                newName: "ChannelMapping_VirtualRegisterType");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_OutputStop",
                table: "Devices",
                newName: "ChannelMapping_OutputStop");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_OutputStart",
                table: "Devices",
                newName: "ChannelMapping_OutputStart");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_OutputRegisterType",
                table: "Devices",
                newName: "ChannelMapping_OutputRegisterType");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_DiscreteStop",
                table: "Devices",
                newName: "ChannelMapping_DiscreteStop");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_DiscreteStart",
                table: "Devices",
                newName: "ChannelMapping_DiscreteStart");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_DiscreteRegisterType",
                table: "Devices",
                newName: "ChannelMapping_DiscreteRegisterType");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_DeviceStop",
                table: "Devices",
                newName: "ChannelMapping_DeviceStop");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_DeviceStart",
                table: "Devices",
                newName: "ChannelMapping_DeviceStart");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_DeviceRegisterType",
                table: "Devices",
                newName: "ChannelMapping_DeviceRegisterType");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_AnalogStop",
                table: "Devices",
                newName: "ChannelMapping_AnalogStop");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_AnalogStart",
                table: "Devices",
                newName: "ChannelMapping_AnalogStart");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_AnalogRegisterType",
                table: "Devices",
                newName: "ChannelMapping_AnalogRegisterType");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_AlertStop",
                table: "Devices",
                newName: "ChannelMapping_AlertStop");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_AlertStart",
                table: "Devices",
                newName: "ChannelMapping_AlertStart");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_AlertRegisterType",
                table: "Devices",
                newName: "ChannelMapping_AlertRegisterType");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_ActionStop",
                table: "Devices",
                newName: "ChannelMapping_ActionStop");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_ActionStart",
                table: "Devices",
                newName: "ChannelMapping_ActionStart");

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_ChannelMapping_ActionRegisterType",
                table: "Devices",
                newName: "ChannelMapping_ActionRegisterType");

            migrationBuilder.RenameColumn(
                name: "ModbusAddress_RegisterType",
                table: "Alerts",
                newName: "MobdusAddress_RegisterType");

            migrationBuilder.RenameColumn(
                name: "ModbusAddress_RegisterLength",
                table: "Alerts",
                newName: "MobdusAddress_RegisterLength");

            migrationBuilder.RenameColumn(
                name: "ModbusAddress_Address",
                table: "Alerts",
                newName: "MobdusAddress_Address");
        }
    }
}
