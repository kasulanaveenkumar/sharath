using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Alter_Table_TemplateSetCustomImageMappings_Rename_Column_IsMandatory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsMandatory",
                table: "TemplateSetCustomImageMappings",
                newName: "IsSkippable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSkippable",
                table: "TemplateSetCustomImageMappings",
                newName: "IsMandatory");
        }
    }
}
