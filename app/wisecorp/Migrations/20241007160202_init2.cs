using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisecorp.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Titles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "nom",
                table: "Roles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "nom",
                table: "Departements",
                newName: "Name");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DisableDate",
                table: "Accounts",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Titles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Roles",
                newName: "nom");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Departements",
                newName: "nom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DisableDate",
                table: "Accounts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
