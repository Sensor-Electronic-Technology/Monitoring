using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class analogthreshold : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RecordThreshold",
                table: "Sensors",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordThreshold",
                table: "Sensors");
        }
    }
}
