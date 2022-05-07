using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_Rename_AppStakeholders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppStakeHolders",
                table: "AppStakeHolders");

            migrationBuilder.RenameTable(
                name: "AppStakeHolders",
                newName: "AppStakeholders");

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppStakeholders",
                table: "AppStakeholders",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppStakeholders",
                table: "AppStakeholders");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "Applications");

            migrationBuilder.RenameTable(
                name: "AppStakeholders",
                newName: "AppStakeHolders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppStakeHolders",
                table: "AppStakeHolders",
                column: "Id");
        }
    }
}
