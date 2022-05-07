using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Add_Tables_TemplateImageHelpImages_TemplateImageOverlayHelpImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InspectionsList",
                columns: table => new
                {
                    BuyerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrokerCompany = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LenderRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationStatus = table.Column<int>(type: "int", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    IsBypassRequested = table.Column<bool>(type: "bit", nullable: false),
                    TotalRecordsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

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
                name: "InspectionsList");

            migrationBuilder.DropTable(
                name: "TemplateImageHelpImages");

            migrationBuilder.DropTable(
                name: "TemplateImageOverlayImages");
        }
    }
}
