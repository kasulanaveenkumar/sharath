using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Add_NewTable_StaCollectorDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaCollectorDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CompanyDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LogoGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserFirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserLastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserMobileNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CollectorAggregatorRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaCollectorDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaCollectorDetails");
        }
    }
}
