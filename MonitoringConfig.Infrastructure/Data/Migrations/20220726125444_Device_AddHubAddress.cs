using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringConfig.Infrastructure.Data.Migrations
{
    public partial class Device_AddHubAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HubAddress",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HubAddress",
                table: "Devices");
        }
    }
}
