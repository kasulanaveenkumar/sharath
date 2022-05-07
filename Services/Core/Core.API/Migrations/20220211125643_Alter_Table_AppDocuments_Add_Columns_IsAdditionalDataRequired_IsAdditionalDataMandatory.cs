using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_AppDocuments_Add_Columns_IsAdditionalDataRequired_IsAdditionalDataMandatory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdditionalDataMandatory",
                table: "AppDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdditionalDataRequired",
                table: "AppDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdditionalDataMandatory",
                table: "AppDocuments");

            migrationBuilder.DropColumn(
                name: "IsAdditionalDataRequired",
                table: "AppDocuments");
        }
    }
}
