using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisecorp.Migrations
{
    public partial class UpdateWork3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeposite",
                table: "Works",
                newName: "IsEnable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnable",
                table: "Works",
                newName: "IsDeposite");
        }
    }
}
