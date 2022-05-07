﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.API.Migrations
{
    public partial class Add_Table_ADTemplateSetLenderPlans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ADTemplateSetLenderPlans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateGuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanGuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LenderCompanyGuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADTemplateSetLenderPlans", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADTemplateSetLenderPlans");
        }
    }
}
