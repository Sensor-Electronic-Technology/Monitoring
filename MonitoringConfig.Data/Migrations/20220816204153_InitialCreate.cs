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
                    FirmwareId = table.Column<int>(type: "int", nullable: false),
                    EmailEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EmailPeriod = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sensor",
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
                    table.PrimaryKey("PK_Sensor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModbusChannelRegisterMap",
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
                    table.PrimaryKey("PK_ModbusChannelRegisterMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModbusChannelRegisterMap_Devices_ModbusDeviceId",
                        column: x => x.ModbusDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModbusConfiguration",
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
                    table.PrimaryKey("PK_ModbusConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModbusConfiguration_Devices_ModbusDeviceId",
                        column: x => x.ModbusDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NetworkConfiguration",
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
                    table.PrimaryKey("PK_NetworkConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkConfiguration_Devices_ModbusDeviceId",
                        column: x => x.ModbusDeviceId,
                        principalTable: "Devices",
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
                        name: "FK_Channels_Sensor_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bypass = table.Column<bool>(type: "bit", nullable: false),
                    BypassResetTime = table.Column<bool>(type: "bit", nullable: false),
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
                name: "AlertLevel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bypass = table.Column<bool>(type: "bit", nullable: false),
                    BypassResetTime = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetPoint = table.Column<double>(type: "float", nullable: true),
                    AnalogAlertId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TriggerOn = table.Column<int>(type: "int", nullable: true),
                    DiscreteAlertId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertLevel_Alerts_AnalogAlertId",
                        column: x => x.AnalogAlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AlertLevel_Alerts_DiscreteAlertId",
                        column: x => x.DiscreteAlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AlertLevelActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlertLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertLevelActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertLevelActions_AlertLevel_AlertLevelId",
                        column: x => x.AlertLevelId,
                        principalTable: "AlertLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertLevelActions_FacilityActions_FacilityActionId",
                        column: x => x.FacilityActionId,
                        principalTable: "FacilityActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionOutputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscreteOutputId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlertLevelActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnLevel = table.Column<int>(type: "int", nullable: false),
                    OffLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionOutputs_AlertLevelActions_AlertLevelActionId",
                        column: x => x.AlertLevelActionId,
                        principalTable: "AlertLevelActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionOutputs_Channels_DiscreteOutputId",
                        column: x => x.DiscreteOutputId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionOutputs_AlertLevelActionId",
                table: "ActionOutputs",
                column: "AlertLevelActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionOutputs_DiscreteOutputId",
                table: "ActionOutputs",
                column: "DiscreteOutputId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevel_AnalogAlertId",
                table: "AlertLevel",
                column: "AnalogAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevel_DiscreteAlertId",
                table: "AlertLevel",
                column: "DiscreteAlertId",
                unique: true,
                filter: "[DiscreteAlertId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevelActions_AlertLevelId",
                table: "AlertLevelActions",
                column: "AlertLevelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevelActions_FacilityActionId",
                table: "AlertLevelActions",
                column: "FacilityActionId");

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
                name: "IX_ModbusChannelRegisterMap_ModbusDeviceId",
                table: "ModbusChannelRegisterMap",
                column: "ModbusDeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModbusConfiguration_ModbusDeviceId",
                table: "ModbusConfiguration",
                column: "ModbusDeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetworkConfiguration_ModbusDeviceId",
                table: "NetworkConfiguration",
                column: "ModbusDeviceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionOutputs");

            migrationBuilder.DropTable(
                name: "ModbusChannelRegisterMap");

            migrationBuilder.DropTable(
                name: "ModbusConfiguration");

            migrationBuilder.DropTable(
                name: "NetworkConfiguration");

            migrationBuilder.DropTable(
                name: "AlertLevelActions");

            migrationBuilder.DropTable(
                name: "AlertLevel");

            migrationBuilder.DropTable(
                name: "FacilityActions");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Sensor");
        }
    }
}
