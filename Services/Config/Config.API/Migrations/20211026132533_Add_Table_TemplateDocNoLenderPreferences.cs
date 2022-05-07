using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Add_Table_TemplateDocNoLenderPreferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateDocNoLenderPreferences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateSetId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateSetPlanId = table.Column<long>(type: "bigint", nullable: false),
                    UserGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Preference = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateDocNoLenderPreferences", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateDocNoLenderPreferences");
        }
    }
}
