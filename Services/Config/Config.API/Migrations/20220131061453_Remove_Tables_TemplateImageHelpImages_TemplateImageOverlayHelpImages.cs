using Microsoft.EntityFrameworkCore.Migrations;

namespace Config.API.Migrations
{
    public partial class Remove_Tables_TemplateImageHelpImages_TemplateImageOverlayHelpImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateImageHelpImages");

            migrationBuilder.DropTable(
                name: "TemplateImageOverlayImages");

            migrationBuilder.CreateTable(
                name: "GetInspectionPlanDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TemplateSetGuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanGuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlanLevel = table.Column<short>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsAppliedAllState = table.Column<bool>(type: "bit", nullable: true),
                    StateId = table.Column<long>(type: "bigint", nullable: false),
                    StateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocId = table.Column<long>(type: "bigint", nullable: false),
                    DocName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocWarningMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocPosition = table.Column<short>(type: "smallint", nullable: false),
                    IsShowAdditionalData = table.Column<bool>(type: "bit", nullable: true),
                    IsAdditionalDataSelected = table.Column<bool>(type: "bit", nullable: true),
                    IsShowDocument = table.Column<bool>(type: "bit", nullable: true),
                    IsDocumentSelected = table.Column<bool>(type: "bit", nullable: true),
                    isDocumentReadOnly = table.Column<bool>(type: "bit", nullable: true),
                    ImageId = table.Column<long>(type: "bigint", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarningMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePosition = table.Column<short>(type: "smallint", nullable: false),
                    IsShowMandatory = table.Column<bool>(type: "bit", nullable: true),
                    IsMandatorySelected = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GetInspectionPlanDetails");

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
                    ImageType = table.Column<short>(type: "smallint", nullable: false),
                    OverlayImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateSetId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateImageOverlayImages", x => x.Id);
                });
        }
    }
}
