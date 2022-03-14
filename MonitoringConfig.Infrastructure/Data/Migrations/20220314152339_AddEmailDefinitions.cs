using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class AddEmailDefinitions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataConfigIteration",
                table: "Devices");

            migrationBuilder.AddColumn<bool>(
                name: "EmailEnabled",
                table: "AlertLevel",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EmailPeriod",
                table: "AlertLevel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailEnabled",
                table: "AlertLevel");

            migrationBuilder.DropColumn(
                name: "EmailPeriod",
                table: "AlertLevel");

            migrationBuilder.AddColumn<int>(
                name: "DataConfigIteration",
                table: "Devices",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }
    }
}
