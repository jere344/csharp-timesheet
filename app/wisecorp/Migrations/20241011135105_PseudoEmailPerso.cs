using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisecorp.Migrations
{
    public partial class PseudoEmailPerso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonalEmail",
                table: "Accounts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Pseudo",
                table: "Accounts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalEmail",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Pseudo",
                table: "Accounts");
        }
    }
}
