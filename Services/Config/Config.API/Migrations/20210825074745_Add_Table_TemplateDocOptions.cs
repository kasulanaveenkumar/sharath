using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Add_Table_TemplateDocOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultSelected",
                table: "TemplateDocImageMappings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "TemplateDocImageMappings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TemplateDocOptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    OptionType = table.Column<short>(type: "smallint", nullable: false),
                    OptionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateDocOptions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateDocOptions");

            migrationBuilder.DropColumn(
                name: "IsDefaultSelected",
                table: "TemplateDocImageMappings");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "TemplateDocImageMappings");
        }
    }
}
