﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_AppActivityLogs_Add_Column_IsWebAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWebAppUser",
                table: "AppActivityLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWebAppUser",
                table: "AppActivityLogs");
        }
    }
}
