using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class updateEmailSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataConfigIteration",
                table: "Devices");

            migrationBuilder.AddColumn<bool>(
                name: "EmailEnabled",
                table: "FacilityActions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EmailPeriod",
                table: "FacilityActions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailEnabled",
                table: "FacilityActions");

            migrationBuilder.DropColumn(
                name: "EmailPeriod",
                table: "FacilityActions");

            migrationBuilder.AddColumn<int>(
                name: "DataConfigIteration",
                table: "Devices",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }
    }
}
