using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Data.Migrations
{
    public partial class Refractor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlaveAddress",
                table: "NetworkConfigurations");

            migrationBuilder.AddColumn<int>(
                name: "SlaveAddress",
                table: "ModbusConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlaveAddress",
                table: "ModbusConfigurations");

            migrationBuilder.AddColumn<int>(
                name: "SlaveAddress",
                table: "NetworkConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
