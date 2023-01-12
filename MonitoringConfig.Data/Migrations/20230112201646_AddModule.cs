using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class AddModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleType = table.Column<int>(type: "int", nullable: false),
                    ChannelCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitorBoxModule",
                columns: table => new
                {
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonitorBoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleSlot = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorBoxModule", x => new { x.ModuleId, x.MonitorBoxId });
                    table.ForeignKey(
                        name: "FK_MonitorBoxModule_Devices_MonitorBoxId",
                        column: x => x.MonitorBoxId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonitorBoxModule_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitorBoxModule_MonitorBoxId",
                table: "MonitorBoxModule",
                column: "MonitorBoxId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitorBoxModule");

            migrationBuilder.DropTable(
                name: "Module");
        }
    }
}
