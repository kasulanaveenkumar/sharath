using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Alter_Table_TemplateSetCustomPlanMappings_Rename_Column_LenderCompanyGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LenderCompanyGuid",
                table: "TemplateSetCustomPlanMappings");

            migrationBuilder.AddColumn<long>(
                name: "LenderCompanyId",
                table: "TemplateSetCustomPlanMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LenderCompanyId",
                table: "TemplateSetCustomPlanMappings");

            migrationBuilder.AddColumn<string>(
                name: "LenderCompanyGuid",
                table: "TemplateSetCustomPlanMappings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
