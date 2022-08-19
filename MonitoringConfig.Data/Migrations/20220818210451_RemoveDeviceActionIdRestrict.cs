using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class RemoveDeviceActionIdRestrict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevels_DeviceActions_DeviceActionId",
                table: "AlertLevels");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceActionId",
                table: "AlertLevels",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevels_DeviceActions_DeviceActionId",
                table: "AlertLevels",
                column: "DeviceActionId",
                principalTable: "DeviceActions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevels_DeviceActions_DeviceActionId",
                table: "AlertLevels");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceActionId",
                table: "AlertLevels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevels_DeviceActions_DeviceActionId",
                table: "AlertLevels",
                column: "DeviceActionId",
                principalTable: "DeviceActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
