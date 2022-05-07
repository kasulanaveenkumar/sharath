using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Alter_Table_TemplateSetCustomDocMappings_TemplateSetCustomImageMappings_Add_Column_StateCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "TemplateSetCustomImageMappings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "TemplateSetCustomDocMappings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "TemplateSetCustomImageMappings");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "TemplateSetCustomDocMappings");
        }
    }
}
