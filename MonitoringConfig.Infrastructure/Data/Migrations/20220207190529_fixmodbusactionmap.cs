using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class fixmodbusactionmap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModbusActionMap_Devices_MonitoringBoxId",
                table: "ModbusActionMap");

            migrationBuilder.DropForeignKey(
                name: "FK_ModbusActionMap_FacilityActions_FacilityActionId",
                table: "ModbusActionMap");

            migrationBuilder.DropIndex(
                name: "IX_ModbusActionMap_FacilityActionId",
                table: "ModbusActionMap");

            migrationBuilder.DropIndex(
                name: "IX_ModbusActionMap_MonitoringBoxId",
                table: "ModbusActionMap");

            migrationBuilder.InsertData(
                table: "FacilityActions",
                columns: new[] { "Id", "ActionName", "ActionType" },
                values: new object[,]
                {
                    { 1, "Okay", 6 },
                    { 2, "Alarm", 5 },
                    { 3, "Warning", 4 },
                    { 4, "SoftWarn", 3 },
                    { 5, "Maintenance", 2 },
                    { 6, "ResetWetFloor", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModbusActionMap_FacilityActionId",
                table: "ModbusActionMap",
                column: "FacilityActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ModbusActionMap_MonitoringBoxId",
                table: "ModbusActionMap",
                column: "MonitoringBoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModbusActionMap_Devices_MonitoringBoxId",
                table: "ModbusActionMap",
                column: "MonitoringBoxId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModbusActionMap_FacilityActions_FacilityActionId",
                table: "ModbusActionMap",
                column: "FacilityActionId",
                principalTable: "FacilityActions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModbusActionMap_Devices_MonitoringBoxId",
                table: "ModbusActionMap");

            migrationBuilder.DropForeignKey(
                name: "FK_ModbusActionMap_FacilityActions_FacilityActionId",
                table: "ModbusActionMap");

            migrationBuilder.DropIndex(
                name: "IX_ModbusActionMap_FacilityActionId",
                table: "ModbusActionMap");

            migrationBuilder.DropIndex(
                name: "IX_ModbusActionMap_MonitoringBoxId",
                table: "ModbusActionMap");

            migrationBuilder.DeleteData(
                table: "FacilityActions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FacilityActions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FacilityActions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FacilityActions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FacilityActions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FacilityActions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.CreateIndex(
                name: "IX_ModbusActionMap_FacilityActionId",
                table: "ModbusActionMap",
                column: "FacilityActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModbusActionMap_MonitoringBoxId",
                table: "ModbusActionMap",
                column: "MonitoringBoxId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModbusActionMap_Devices_MonitoringBoxId",
                table: "ModbusActionMap",
                column: "MonitoringBoxId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModbusActionMap_FacilityActions_FacilityActionId",
                table: "ModbusActionMap",
                column: "FacilityActionId",
                principalTable: "FacilityActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
