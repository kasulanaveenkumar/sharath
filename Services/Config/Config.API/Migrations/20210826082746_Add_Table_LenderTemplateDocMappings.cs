using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Add_Table_LenderTemplateDocMappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LenderTemplateDocImageMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LenderCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateSetId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateImageId = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsSkippable = table.Column<bool>(type: "bit", nullable: false),
                    NotRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderTemplateDocImageMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LenderTemplateSetDocMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LenderCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateSetId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    UploadOptions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderTemplateSetDocMappings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LenderTemplateDocImageMappings");

            migrationBuilder.DropTable(
                name: "LenderTemplateSetDocMappings");
        }
    }
}
