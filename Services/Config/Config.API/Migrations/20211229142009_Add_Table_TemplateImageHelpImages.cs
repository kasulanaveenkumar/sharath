using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Add_Table_TemplateImageHelpImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateImageHelpImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HelpImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageType = table.Column<short>(type: "smallint", nullable: false),
                    TemplateSetId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateImageHelpImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateImageOverlayImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OverlayImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageType = table.Column<short>(type: "smallint", nullable: false),
                    TemplateSetId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateImageOverlayImages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateImageHelpImages");

            migrationBuilder.DropTable(
                name: "TemplateImageOverlayImages");
        }
    }
}
