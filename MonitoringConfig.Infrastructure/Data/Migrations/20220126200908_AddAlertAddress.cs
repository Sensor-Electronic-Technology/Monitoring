using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class AddAlertAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlertAddress_Address",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlertAddress_RegisterLength",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlertAddress_RegisterType",
                table: "Channels",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertAddress_Address",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "AlertAddress_RegisterLength",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "AlertAddress_RegisterType",
                table: "Channels");
        }
    }
}
