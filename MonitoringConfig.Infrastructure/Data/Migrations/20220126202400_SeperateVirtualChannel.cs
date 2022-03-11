using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class SeperateVirtualChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VirtualAlertId",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_VirtualAlertId",
                table: "Channels",
                column: "VirtualAlertId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Alerts_VirtualAlertId",
                table: "Channels",
                column: "VirtualAlertId",
                principalTable: "Alerts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Alerts_VirtualAlertId",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_VirtualAlertId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "VirtualAlertId",
                table: "Channels");
        }
    }
}
