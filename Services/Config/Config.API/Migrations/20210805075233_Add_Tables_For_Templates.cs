using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Add_Tables_For_Templates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateDocImageMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateImageId = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateDocImageMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AdditionalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ImageType = table.Column<short>(type: "smallint", nullable: false),
                    DocGroup = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateSetDocMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateSetId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    Position = table.Column<short>(type: "smallint", nullable: false),
                    IsDefaultSelected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateSetDocMappings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateDocImageMappings");

            migrationBuilder.DropTable(
                name: "TemplateDocuments");

            migrationBuilder.DropTable(
                name: "TemplateImages");

            migrationBuilder.DropTable(
                name: "TemplateSetDocMappings");
        }
    }
}
