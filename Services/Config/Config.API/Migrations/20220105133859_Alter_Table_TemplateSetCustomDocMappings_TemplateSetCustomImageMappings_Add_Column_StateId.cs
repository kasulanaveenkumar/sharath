using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Alter_Table_TemplateSetCustomDocMappings_TemplateSetCustomImageMappings_Add_Column_StateId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "TemplateSetCustomImageMappings");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "TemplateSetCustomDocMappings");

            migrationBuilder.AddColumn<long>(
                name: "StateId",
                table: "TemplateSetCustomImageMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StateId",
                table: "TemplateSetCustomDocMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateId",
                table: "TemplateSetCustomImageMappings");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "TemplateSetCustomDocMappings");

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
    }
}
