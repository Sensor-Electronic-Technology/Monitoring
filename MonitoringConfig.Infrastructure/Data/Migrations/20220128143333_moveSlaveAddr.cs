using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class moveSlaveAddr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_SlaveAddress",
                table: "Devices",
                newName: "NetworkConfiguration_ModbusConfig_SlaveAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NetworkConfiguration_ModbusConfig_SlaveAddress",
                table: "Devices",
                newName: "NetworkConfiguration_SlaveAddress");
        }
    }
}
