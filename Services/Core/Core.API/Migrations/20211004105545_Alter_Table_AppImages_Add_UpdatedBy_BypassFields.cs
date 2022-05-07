using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_AppImages_Add_UpdatedBy_BypassFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BypassReason",
                table: "AppImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBypassRequested",
                table: "AppImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "AppImages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "AppImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<short>(
                name: "UserType",
                table: "AppImages",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BypassReason",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "IsBypassRequested",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "AppImages");
        }
    }
}
