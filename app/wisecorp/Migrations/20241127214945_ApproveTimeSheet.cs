using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisecorp.Migrations
{
    public partial class ApproveTimeSheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnable",
                table: "Works",
                newName: "IsSubmitted");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Works",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "Works",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                table: "Works",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "RejectedReason",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "IsSubmitted",
                table: "Works",
                newName: "IsEnable");
        }
    }
}
