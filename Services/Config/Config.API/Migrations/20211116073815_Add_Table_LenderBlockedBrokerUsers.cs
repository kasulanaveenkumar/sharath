using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Add_Table_LenderBlockedBrokerUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LenderBlockedBrokerUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrokerCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    LenderCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    UserGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderBlockedBrokerUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LenderBlockedBrokerUsers");
        }
    }
}
