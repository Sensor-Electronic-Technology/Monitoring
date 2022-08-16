using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class UpdateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevel_Alerts_AnalogAlertId",
                table: "AlertLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevel_Alerts_DiscreteAlertId",
                table: "AlertLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevelActions_AlertLevel_AlertLevelId",
                table: "AlertLevelActions");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Sensor_SensorId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_ModbusChannelRegisterMap_Devices_ModbusDeviceId",
                table: "ModbusChannelRegisterMap");

            migrationBuilder.DropForeignKey(
                name: "FK_ModbusConfiguration_Devices_ModbusDeviceId",
                table: "ModbusConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_NetworkConfiguration_Devices_ModbusDeviceId",
                table: "NetworkConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sensor",
                table: "Sensor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NetworkConfiguration",
                table: "NetworkConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModbusConfiguration",
                table: "ModbusConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModbusChannelRegisterMap",
                table: "ModbusChannelRegisterMap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlertLevel",
                table: "AlertLevel");

            migrationBuilder.RenameTable(
                name: "Sensor",
                newName: "Sensors");

            migrationBuilder.RenameTable(
                name: "NetworkConfiguration",
                newName: "NetworkConfigurations");

            migrationBuilder.RenameTable(
                name: "ModbusConfiguration",
                newName: "ModbusConfigurations");

            migrationBuilder.RenameTable(
                name: "ModbusChannelRegisterMap",
                newName: "ModbusChannelRegisterMaps");

            migrationBuilder.RenameTable(
                name: "AlertLevel",
                newName: "AlertLevels");

            migrationBuilder.RenameIndex(
                name: "IX_NetworkConfiguration_ModbusDeviceId",
                table: "NetworkConfigurations",
                newName: "IX_NetworkConfigurations_ModbusDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_ModbusConfiguration_ModbusDeviceId",
                table: "ModbusConfigurations",
                newName: "IX_ModbusConfigurations_ModbusDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_ModbusChannelRegisterMap_ModbusDeviceId",
                table: "ModbusChannelRegisterMaps",
                newName: "IX_ModbusChannelRegisterMaps_ModbusDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_AlertLevel_DiscreteAlertId",
                table: "AlertLevels",
                newName: "IX_AlertLevels_DiscreteAlertId");

            migrationBuilder.RenameIndex(
                name: "IX_AlertLevel_AnalogAlertId",
                table: "AlertLevels",
                newName: "IX_AlertLevels_AnalogAlertId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NetworkConfigurations",
                table: "NetworkConfigurations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModbusConfigurations",
                table: "ModbusConfigurations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModbusChannelRegisterMaps",
                table: "ModbusChannelRegisterMaps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlertLevels",
                table: "AlertLevels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevelActions_AlertLevels_AlertLevelId",
                table: "AlertLevelActions",
                column: "AlertLevelId",
                principalTable: "AlertLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevels_Alerts_AnalogAlertId",
                table: "AlertLevels",
                column: "AnalogAlertId",
                principalTable: "Alerts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevels_Alerts_DiscreteAlertId",
                table: "AlertLevels",
                column: "DiscreteAlertId",
                principalTable: "Alerts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Sensors_SensorId",
                table: "Channels",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModbusChannelRegisterMaps_Devices_ModbusDeviceId",
                table: "ModbusChannelRegisterMaps",
                column: "ModbusDeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModbusConfigurations_Devices_ModbusDeviceId",
                table: "ModbusConfigurations",
                column: "ModbusDeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NetworkConfigurations_Devices_ModbusDeviceId",
                table: "NetworkConfigurations",
                column: "ModbusDeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevelActions_AlertLevels_AlertLevelId",
                table: "AlertLevelActions");

            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevels_Alerts_AnalogAlertId",
                table: "AlertLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevels_Alerts_DiscreteAlertId",
                table: "AlertLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Sensors_SensorId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_ModbusChannelRegisterMaps_Devices_ModbusDeviceId",
                table: "ModbusChannelRegisterMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_ModbusConfigurations_Devices_ModbusDeviceId",
                table: "ModbusConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_NetworkConfigurations_Devices_ModbusDeviceId",
                table: "NetworkConfigurations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NetworkConfigurations",
                table: "NetworkConfigurations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModbusConfigurations",
                table: "ModbusConfigurations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModbusChannelRegisterMaps",
                table: "ModbusChannelRegisterMaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlertLevels",
                table: "AlertLevels");

            migrationBuilder.RenameTable(
                name: "Sensors",
                newName: "Sensor");

            migrationBuilder.RenameTable(
                name: "NetworkConfigurations",
                newName: "NetworkConfiguration");

            migrationBuilder.RenameTable(
                name: "ModbusConfigurations",
                newName: "ModbusConfiguration");

            migrationBuilder.RenameTable(
                name: "ModbusChannelRegisterMaps",
                newName: "ModbusChannelRegisterMap");

            migrationBuilder.RenameTable(
                name: "AlertLevels",
                newName: "AlertLevel");

            migrationBuilder.RenameIndex(
                name: "IX_NetworkConfigurations_ModbusDeviceId",
                table: "NetworkConfiguration",
                newName: "IX_NetworkConfiguration_ModbusDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_ModbusConfigurations_ModbusDeviceId",
                table: "ModbusConfiguration",
                newName: "IX_ModbusConfiguration_ModbusDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_ModbusChannelRegisterMaps_ModbusDeviceId",
                table: "ModbusChannelRegisterMap",
                newName: "IX_ModbusChannelRegisterMap_ModbusDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_AlertLevels_DiscreteAlertId",
                table: "AlertLevel",
                newName: "IX_AlertLevel_DiscreteAlertId");

            migrationBuilder.RenameIndex(
                name: "IX_AlertLevels_AnalogAlertId",
                table: "AlertLevel",
                newName: "IX_AlertLevel_AnalogAlertId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sensor",
                table: "Sensor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NetworkConfiguration",
                table: "NetworkConfiguration",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModbusConfiguration",
                table: "ModbusConfiguration",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModbusChannelRegisterMap",
                table: "ModbusChannelRegisterMap",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlertLevel",
                table: "AlertLevel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevel_Alerts_AnalogAlertId",
                table: "AlertLevel",
                column: "AnalogAlertId",
                principalTable: "Alerts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevel_Alerts_DiscreteAlertId",
                table: "AlertLevel",
                column: "DiscreteAlertId",
                principalTable: "Alerts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevelActions_AlertLevel_AlertLevelId",
                table: "AlertLevelActions",
                column: "AlertLevelId",
                principalTable: "AlertLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Sensor_SensorId",
                table: "Channels",
                column: "SensorId",
                principalTable: "Sensor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModbusChannelRegisterMap_Devices_ModbusDeviceId",
                table: "ModbusChannelRegisterMap",
                column: "ModbusDeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModbusConfiguration_Devices_ModbusDeviceId",
                table: "ModbusConfiguration",
                column: "ModbusDeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NetworkConfiguration_Devices_ModbusDeviceId",
                table: "NetworkConfiguration",
                column: "ModbusDeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
