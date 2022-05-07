using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_AppDocuments_Remove_Column_BankStatementUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankStatementUrl",
                table: "AppDocuments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankStatementUrl",
                table: "AppDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
