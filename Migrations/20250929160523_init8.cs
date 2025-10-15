using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareVolt.Migrations
{
    public partial class init8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Bookings");

            migrationBuilder.AddColumn<decimal>(
                name: "BatterySize",
                table: "Bookings",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SizeType",
                table: "Bookings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatterySize",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SizeType",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
