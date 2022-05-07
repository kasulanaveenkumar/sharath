using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Alter_Table_ADUsers_Add_Column_IsPrimary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ADUsers",
                newName: "IsPrimary");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPrimary",
                table: "ADUsers",
                newName: "IsDeleted");
        }
    }
}
