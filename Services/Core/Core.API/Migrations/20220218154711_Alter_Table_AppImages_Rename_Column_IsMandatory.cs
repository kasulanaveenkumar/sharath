using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_AppImages_Rename_Column_IsMandatory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsMandatory",
                table: "AppImages",
                newName: "IsSkippable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSkippable",
                table: "AppImages",
                newName: "IsMandatory");
        }
    }
}
