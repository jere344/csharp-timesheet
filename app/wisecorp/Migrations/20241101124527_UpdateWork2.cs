using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisecorp.Migrations
{
    public partial class UpdateWork2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeposite",
                table: "Works",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeposite",
                table: "Works");
        }
    }
}
