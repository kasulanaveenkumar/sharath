using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Alter_Table_PaymentLogs_Add_Columns_IsPaidByCard_IsInvoiced_InvoiceCompanyGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceCompanyGuid",
                table: "PaymentLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiced",
                table: "PaymentLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidByCard",
                table: "PaymentLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceCompanyGuid",
                table: "PaymentLogs");

            migrationBuilder.DropColumn(
                name: "IsInvoiced",
                table: "PaymentLogs");

            migrationBuilder.DropColumn(
                name: "IsPaidByCard",
                table: "PaymentLogs");
        }
    }
}
