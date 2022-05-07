using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_ADUsers_Add_Column_IsPrimary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ADUsers",
                newName: "IsPrimary");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ADUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "ADUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "ADUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ADUsers");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "ADUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ADUsers");

            migrationBuilder.RenameColumn(
                name: "IsPrimary",
                table: "ADUsers",
                newName: "IsDeleted");
        }
    }
}
