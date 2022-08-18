using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReadInterval = table.Column<int>(type: "int", nullable: false),
                    SaveInterval = table.Column<int>(type: "int", nullable: false),
                    Database = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HubAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EmailPeriod = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordThreshold = table.Column<double>(type: "float", nullable: false),
                    Slope = table.Column<double>(type: "float", nullable: false),
                    Offset = table.Column<double>(type: "float", nullable: false),
                    Factor = table.Column<double>(type: "float", nullable: false),
                    Units = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YAxisMin = table.Column<int>(type: "int", nullable: false),
                    YAxitMax = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModbusChannelRegisterMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModbusDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlertRegisterType = table.Column<int>(type: "int", nullable: false),
                    AlertStart = table.Column<int>(type: "int", nullable: false),
                    AlertStop = table.Column<int>(type: "int", nullable: false),
                    AnalogRegisterType = table.Column<int>(type: "int", nullable: false),
                    AnalogStart = table.Column<int>(type: "int", nullable: false),
                    AnalogStop = table.Column<int>(type: "int", nullable: false),
                    DiscreteRegisterType = table.Column<int>(type: "int", nullable: false),
                    DiscreteStart = table.Column<int>(type: "int", nullable: false),
                    DiscreteStop = table.Column<int>(type: "int", nullable: false),
                    VirtualRegisterType = table.Column<int>(type: "int", nullable: false),
                    VirtualStart = table.Column<int>(type: "int", nullable: false),
                    VirtualStop = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusChannelRegisterMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModbusChannelRegisterMaps_Devices_ModbusDeviceId",
                        column: x => x.ModbusDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModbusConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscreteInputs = table.Column<int>(type: "int", nullable: false),
                    InputRegisters = table.Column<int>(type: "int", nullable: false),
                    HoldingRegisters = table.Column<int>(type: "int", nullable: false),
                    Coils = table.Column<int>(type: "int", nullable: false),
                    ModbusDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModbusConfigurations_Devices_ModbusDeviceId",
                        column: x => x.ModbusDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NetworkConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gateway = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    SlaveAddress = table.Column<int>(type: "int", nullable: false),
                    ModbusDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkConfigurations_Devices_ModbusDeviceId",
                        column: x => x.ModbusDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirmwareId = table.Column<int>(type: "int", nullable: false),
                    MonitorBoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceActions_Devices_MonitorBoxId",
                        column: x => x.MonitorBoxId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceActions_FacilityActions_FacilityActionId",
                        column: x => x.FacilityActionId,
                        principalTable: "FacilityActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemChannel = table.Column<int>(type: "int", nullable: false),
                    Connected = table.Column<bool>(type: "bit", nullable: false),
                    Bypass = table.Column<bool>(type: "bit", nullable: false),
                    Display = table.Column<bool>(type: "bit", nullable: false),
                    ModbusAddress_Address = table.Column<int>(type: "int", nullable: true),
                    ModbusAddress_RegisterLength = table.Column<int>(type: "int", nullable: true),
                    ModbusAddress_RegisterType = table.Column<int>(type: "int", nullable: true),
                    ChannelAddress_Channel = table.Column<int>(type: "int", nullable: true),
                    ChannelAddress_ModuleSlot = table.Column<int>(type: "int", nullable: true),
                    ModbusDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SensorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartState = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_Devices_ModbusDeviceId",
                        column: x => x.ModbusDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Channels_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionOutputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscreteOutputId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnLevel = table.Column<int>(type: "int", nullable: false),
                    OffLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionOutputs_Channels_DiscreteOutputId",
                        column: x => x.DiscreteOutputId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionOutputs_DeviceActions_DeviceActionId",
                        column: x => x.DeviceActionId,
                        principalTable: "DeviceActions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bypass = table.Column<bool>(type: "bit", nullable: false),
                    BypassResetTime = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    ModbusAddress_Address = table.Column<int>(type: "int", nullable: true),
                    ModbusAddress_RegisterLength = table.Column<int>(type: "int", nullable: true),
                    ModbusAddress_RegisterType = table.Column<int>(type: "int", nullable: true),
                    InputChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Channels_InputChannelId",
                        column: x => x.InputChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bypass = table.Column<bool>(type: "bit", nullable: false),
                    BypassResetTime = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    AlertId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetPoint = table.Column<double>(type: "float", nullable: true),
                    TriggerOn = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertLevels_Alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AlertLevels_Alerts_AlertId1",
                        column: x => x.AlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AlertLevels_DeviceActions_DeviceActionId",
                        column: x => x.DeviceActionId,
                        principalTable: "DeviceActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionOutputs_DeviceActionId",
                table: "ActionOutputs",
                column: "DeviceActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionOutputs_DiscreteOutputId",
                table: "ActionOutputs",
                column: "DiscreteOutputId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevels_AlertId",
                table: "AlertLevels",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevels_AlertId1",
                table: "AlertLevels",
                column: "AlertId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevels_DeviceActionId",
                table: "AlertLevels",
                column: "DeviceActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_InputChannelId",
                table: "Alerts",
                column: "InputChannelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ModbusDeviceId",
                table: "Channels",
                column: "ModbusDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_SensorId",
                table: "Channels",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceActions_FacilityActionId",
                table: "DeviceActions",
                column: "FacilityActionId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceActions_MonitorBoxId",
                table: "DeviceActions",
                column: "MonitorBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_ModbusChannelRegisterMaps_ModbusDeviceId",
                table: "ModbusChannelRegisterMaps",
                column: "ModbusDeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModbusConfigurations_ModbusDeviceId",
                table: "ModbusConfigurations",
                column: "ModbusDeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetworkConfigurations_ModbusDeviceId",
                table: "NetworkConfigurations",
                column: "ModbusDeviceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionOutputs");

            migrationBuilder.DropTable(
                name: "AlertLevels");

            migrationBuilder.DropTable(
                name: "ModbusChannelRegisterMaps");

            migrationBuilder.DropTable(
                name: "ModbusConfigurations");

            migrationBuilder.DropTable(
                name: "NetworkConfigurations");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "DeviceActions");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "FacilityActions");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Sensors");
        }
    }
}
