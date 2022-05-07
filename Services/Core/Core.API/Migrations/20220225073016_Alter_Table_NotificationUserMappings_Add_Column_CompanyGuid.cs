using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_NotificationUserMappings_Add_Column_CompanyGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "NotificationUserMappings");

            migrationBuilder.AddColumn<string>(
                name: "CompanyGuid",
                table: "NotificationUserMappings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyGuid",
                table: "NotificationUserMappings");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "NotificationUserMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
