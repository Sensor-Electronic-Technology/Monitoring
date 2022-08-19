using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class ModifyALert_AlertLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevels_Alerts_AlertId",
                table: "AlertLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevels_Alerts_AlertId1",
                table: "AlertLevels");

            migrationBuilder.DropIndex(
                name: "IX_AlertLevels_AlertId",
                table: "AlertLevels");

            migrationBuilder.DropIndex(
                name: "IX_AlertLevels_AlertId1",
                table: "AlertLevels");

            migrationBuilder.DropColumn(
                name: "AlertId",
                table: "AlertLevels");

            migrationBuilder.AddColumn<Guid>(
                name: "AnalogAlertId",
                table: "AlertLevels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DiscreteAlertId",
                table: "AlertLevels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevels_AnalogAlertId",
                table: "AlertLevels",
                column: "AnalogAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevels_DiscreteAlertId",
                table: "AlertLevels",
                column: "DiscreteAlertId",
                unique: true,
                filter: "[DiscreteAlertId] IS NOT NULL");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevels_Alerts_AnalogAlertId",
                table: "AlertLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_AlertLevels_Alerts_DiscreteAlertId",
                table: "AlertLevels");

            migrationBuilder.DropIndex(
                name: "IX_AlertLevels_AnalogAlertId",
                table: "AlertLevels");

            migrationBuilder.DropIndex(
                name: "IX_AlertLevels_DiscreteAlertId",
                table: "AlertLevels");

            migrationBuilder.DropColumn(
                name: "AnalogAlertId",
                table: "AlertLevels");

            migrationBuilder.DropColumn(
                name: "DiscreteAlertId",
                table: "AlertLevels");

            migrationBuilder.AddColumn<Guid>(
                name: "AlertId",
                table: "AlertLevels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevels_AlertId",
                table: "AlertLevels",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertLevels_AlertId1",
                table: "AlertLevels",
                column: "AlertId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevels_Alerts_AlertId",
                table: "AlertLevels",
                column: "AlertId",
                principalTable: "Alerts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertLevels_Alerts_AlertId1",
                table: "AlertLevels",
                column: "AlertId",
                principalTable: "Alerts",
                principalColumn: "Id");
        }
    }
}
