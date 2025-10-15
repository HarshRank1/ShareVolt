using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareVolt.Migrations
{
    public partial class init10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Speed",
                table: "Chargers");

            migrationBuilder.AddColumn<int>(
                name: "ChargerSpeed",
                table: "Chargers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargerSpeed",
                table: "Chargers");

            migrationBuilder.AddColumn<int>(
                name: "Speed",
                table: "Chargers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
