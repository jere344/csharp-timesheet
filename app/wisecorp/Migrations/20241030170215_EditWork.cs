using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisecorp.Migrations
{
    public partial class EditWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_Accounts_AccountId",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "HourWorked",
                table: "Works",
                newName: "HourWorkedWed");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Works",
                newName: "WeekStartDate");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Works",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "HourWorkedFri",
                table: "Works",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HourWorkedMon",
                table: "Works",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HourWorkedSat",
                table: "Works",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HourWorkedSun",
                table: "Works",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HourWorkedThur",
                table: "Works",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HourWorkedTue",
                table: "Works",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Accounts_AccountId",
                table: "Works",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_Accounts_AccountId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "HourWorkedFri",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "HourWorkedMon",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "HourWorkedSat",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "HourWorkedSun",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "HourWorkedThur",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "HourWorkedTue",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "WeekStartDate",
                table: "Works",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "HourWorkedWed",
                table: "Works",
                newName: "HourWorked");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Works",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Accounts_AccountId",
                table: "Works",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
