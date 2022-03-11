using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class AddModbusActionMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModbusAddress_Address",
                table: "FacilityActions");

            migrationBuilder.DropColumn(
                name: "ModbusAddress_RegisterLength",
                table: "FacilityActions");

            migrationBuilder.DropColumn(
                name: "ModbusAddress_RegisterType",
                table: "FacilityActions");

            migrationBuilder.CreateTable(
                name: "ModbusActionMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonitoringBoxId = table.Column<int>(type: "int", nullable: false),
                    FacilityActionId = table.Column<int>(type: "int", nullable: false),
                    ModbusAddress_Address = table.Column<int>(type: "int", nullable: true),
                    ModbusAddress_RegisterLength = table.Column<int>(type: "int", nullable: true),
                    ModbusAddress_RegisterType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusActionMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModbusActionMap_Devices_MonitoringBoxId",
                        column: x => x.MonitoringBoxId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModbusActionMap_FacilityActions_FacilityActionId",
                        column: x => x.FacilityActionId,
                        principalTable: "FacilityActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModbusActionMap");

            migrationBuilder.AddColumn<int>(
                name: "ModbusAddress_Address",
                table: "FacilityActions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModbusAddress_RegisterLength",
                table: "FacilityActions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModbusAddress_RegisterType",
                table: "FacilityActions",
                type: "int",
                nullable: true);
        }
    }
}
