using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_AppStakeholders_Modify_UserGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppStakeholders");

            migrationBuilder.AddColumn<string>(
                name: "UserGuid",
                table: "AppStakeholders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserGuid",
                table: "AppStakeholders");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "AppStakeholders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
