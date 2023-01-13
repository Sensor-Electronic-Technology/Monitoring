using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class BoxModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BoxModuleId",
                table: "Channels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleType = table.Column<int>(type: "int", nullable: false),
                    ChannelCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoxModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonitorBoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleSlot = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoxModules_Devices_MonitorBoxId",
                        column: x => x.MonitorBoxId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoxModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_BoxModuleId",
                table: "Channels",
                column: "BoxModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxModules_ModuleId",
                table: "BoxModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxModules_MonitorBoxId",
                table: "BoxModules",
                column: "MonitorBoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_BoxModules_BoxModuleId",
                table: "Channels",
                column: "BoxModuleId",
                principalTable: "BoxModules",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_BoxModules_BoxModuleId",
                table: "Channels");

            migrationBuilder.DropTable(
                name: "BoxModules");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropIndex(
                name: "IX_Channels_BoxModuleId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "BoxModuleId",
                table: "Channels");
        }
    }
}
