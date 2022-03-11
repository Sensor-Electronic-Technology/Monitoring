using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class AlertRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Channels_AnalogInputId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Channels_DiscreteInputId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_FacilityActions_FacilityActionId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Alerts_VirtualAlertId",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_VirtualAlertId",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_AnalogInputId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_DiscreteInputId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_FacilityActionId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "AlertAddress_Address",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "AlertAddress_RegisterLength",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "AlertAddress_RegisterType",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ChannelState",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "VirtualAlertId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "SetPoint",
                table: "Alerts");

            migrationBuilder.RenameColumn(
                name: "TriggerOn",
                table: "Alerts",
                newName: "MobdusAddress_RegisterType");

            migrationBuilder.RenameColumn(
                name: "FacilityActionId",
                table: "Alerts",
                newName: "MobdusAddress_RegisterLength");

            migrationBuilder.RenameColumn(
                name: "DiscreteInputId",
                table: "Alerts",
                newName: "MobdusAddress_Address");

            migrationBuilder.RenameColumn(
                name: "AnalogInputId",
                table: "Alerts",
                newName: "InputChannelId");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Alerts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AlertLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityActionId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetPoint = table.Column<double>(type: "float", nullable: true),
                    AnalogAlertId = table.Column<int>(type: "int", nullable: true),
                    TriggerOn = table.Column<int>(type: "int", nullable: true),
                    DiscrteAlertId = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_AlertLevel_Alerts_DiscrteAlertId",
                        column: x => x.DiscrteAlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AlertLevel_FacilityActions_FacilityActionId",
                        column: x => x.FacilityActionId,
                        principalTable: "FacilityActions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_InputChannelId",
                table: "Alerts",
                column: "InputChannelId",
                unique: true,
                filter: "[InputChannelId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevel_AnalogAlertId",
                table: "AlertLevel",
                column: "AnalogAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevel_DiscrteAlertId",
                table: "AlertLevel",
                column: "DiscrteAlertId",
                unique: true,
                filter: "[DiscrteAlertId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevel_FacilityActionId",
                table: "AlertLevel",
                column: "FacilityActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Channels_InputChannelId",
                table: "Alerts",
                column: "InputChannelId",
                principalTable: "Channels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Channels_InputChannelId",
                table: "Alerts");

            migrationBuilder.DropTable(
                name: "AlertLevel");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_InputChannelId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Alerts");

            migrationBuilder.RenameColumn(
                name: "MobdusAddress_RegisterType",
                table: "Alerts",
                newName: "TriggerOn");

            migrationBuilder.RenameColumn(
                name: "MobdusAddress_RegisterLength",
                table: "Alerts",
                newName: "FacilityActionId");

            migrationBuilder.RenameColumn(
                name: "MobdusAddress_Address",
                table: "Alerts",
                newName: "DiscreteInputId");

            migrationBuilder.RenameColumn(
                name: "InputChannelId",
                table: "Alerts",
                newName: "AnalogInputId");

            migrationBuilder.AddColumn<int>(
                name: "AlertAddress_Address",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlertAddress_RegisterLength",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlertAddress_RegisterType",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelState",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VirtualAlertId",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SetPoint",
                table: "Alerts",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_VirtualAlertId",
                table: "Channels",
                column: "VirtualAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_AnalogInputId",
                table: "Alerts",
                column: "AnalogInputId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DiscreteInputId",
                table: "Alerts",
                column: "DiscreteInputId",
                unique: true,
                filter: "[DiscreteInputId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_FacilityActionId",
                table: "Alerts",
                column: "FacilityActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Channels_AnalogInputId",
                table: "Alerts",
                column: "AnalogInputId",
                principalTable: "Channels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Channels_DiscreteInputId",
                table: "Alerts",
                column: "DiscreteInputId",
                principalTable: "Channels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_FacilityActions_FacilityActionId",
                table: "Alerts",
                column: "FacilityActionId",
                principalTable: "FacilityActions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Alerts_VirtualAlertId",
                table: "Channels",
                column: "VirtualAlertId",
                principalTable: "Alerts",
                principalColumn: "Id");
        }
    }
}
