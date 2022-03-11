using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class AddRegisterToFacilityAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
