using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class AddHubName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HubName",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HubName",
                table: "Devices");
        }
    }
}
