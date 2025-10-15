using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareVolt.Migrations
{
    public partial class init11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EditUserViewModel");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Chargers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Chargers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Chargers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Chargers");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Chargers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Chargers");

            migrationBuilder.CreateTable(
                name: "EditUserViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditUserViewModel", x => x.Id);
                });
        }
    }
}
