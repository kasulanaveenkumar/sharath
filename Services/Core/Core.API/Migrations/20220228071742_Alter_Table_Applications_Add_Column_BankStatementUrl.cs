using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_Applications_Add_Column_BankStatementUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankStatementUrl",
                table: "Applications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankStatementUrl",
                table: "Applications");
        }
    }
}
