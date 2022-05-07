using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Add_Table_TemplateSetCustomDocMappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateSetCustomDocMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomPlanId = table.Column<long>(type: "bigint", nullable: false),
                    StateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    IsAdditionalDataRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsAdditionalDataMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateSetCustomDocMappings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateSetCustomDocMappings");
        }
    }
}
