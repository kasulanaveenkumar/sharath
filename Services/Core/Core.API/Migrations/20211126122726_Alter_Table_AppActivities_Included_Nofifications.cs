using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_AppActivities_Included_Nofifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotificationId",
                table: "NotificationUserMappings",
                newName: "AppActivityId");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabledForNotifications",
                table: "AppActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NotificationDescription",
                table: "AppActivities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotificationGuid",
                table: "AppActivities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabledForNotifications",
                table: "AppActivities");

            migrationBuilder.DropColumn(
                name: "NotificationDescription",
                table: "AppActivities");

            migrationBuilder.DropColumn(
                name: "NotificationGuid",
                table: "AppActivities");

            migrationBuilder.RenameColumn(
                name: "AppActivityId",
                table: "NotificationUserMappings",
                newName: "NotificationId");
        }
    }
}
