using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_Added_Integration_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAPIIntegrationEnabled",
                table: "LenderConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIllionIntegrationEnabled",
                table: "LenderConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAPIIntegrationEnabled",
                table: "LenderConfigurations");

            migrationBuilder.DropColumn(
                name: "IsIllionIntegrationEnabled",
                table: "LenderConfigurations");
        }
    }
}
