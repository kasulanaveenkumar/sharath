using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppActivities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppActivityLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppActivityId = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    ProcessedBy = table.Column<long>(type: "bigint", nullable: false),
                    ProcessedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankStatementUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocStatus = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppImageReasons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    AppImageId = table.Column<long>(type: "bigint", nullable: false),
                    ReasonId = table.Column<long>(type: "bigint", nullable: false),
                    ReasonType = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppImageReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    AppDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    DocGroup = table.Column<short>(type: "smallint", nullable: false),
                    ImageType = table.Column<short>(type: "smallint", nullable: false),
                    ImageInternalStatus = table.Column<short>(type: "smallint", nullable: false),
                    ImageStatus = table.Column<short>(type: "smallint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    SizeInKb = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ImageData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherFlagReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OtherRejectReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateSetGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RefNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalRefNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LenderCompanyGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BrokerCompanyGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicationStatus = table.Column<short>(type: "smallint", nullable: false),
                    PurgeStatus = table.Column<short>(type: "smallint", nullable: false),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppStakeHolders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsOwner = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppStakeHolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SurName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Role = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LenderConfigurations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LenderCompanyGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AdditionalTnC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectInfoStatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsNonOwnerAllowed = table.Column<bool>(type: "bit", nullable: false),
                    IsRoadworthyAllowed = table.Column<bool>(type: "bit", nullable: false),
                    IsBSAllowed = table.Column<bool>(type: "bit", nullable: false),
                    AllowViewBS = table.Column<bool>(type: "bit", nullable: false),
                    IsReportRequired = table.Column<bool>(type: "bit", nullable: false),
                    ReportEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BypassHourMeter = table.Column<bool>(type: "bit", nullable: false),
                    IsSalesInvoiceRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsByPassSalesInvoiceAllowed = table.Column<bool>(type: "bit", nullable: false),
                    IsAdditionalDataRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsByPassAdditionalDataAllowed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReasonMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReasonId = table.Column<long>(type: "bigint", nullable: false),
                    ImageType = table.Column<short>(type: "smallint", nullable: false),
                    ReasonType = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reasons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppActivities");

            migrationBuilder.DropTable(
                name: "AppActivityLogs");

            migrationBuilder.DropTable(
                name: "AppDocuments");

            migrationBuilder.DropTable(
                name: "AppImageReasons");

            migrationBuilder.DropTable(
                name: "AppImages");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "AppStakeHolders");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "LenderConfigurations");

            migrationBuilder.DropTable(
                name: "ReasonMappings");

            migrationBuilder.DropTable(
                name: "Reasons");
        }
    }
}
