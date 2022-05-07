using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_AppUsers_Add_Column_UserGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedBy",
                table: "AppActivityLogs",
                newName: "ApplicationId");

            migrationBuilder.AddColumn<string>(
                name: "UserGuid",
                table: "AppUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserGuid",
                table: "AppActivityLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserGuid",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "UserGuid",
                table: "AppActivityLogs");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "AppActivityLogs",
                newName: "ProcessedBy");
        }
    }
}
