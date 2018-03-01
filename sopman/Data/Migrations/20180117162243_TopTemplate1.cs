using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class TopTemplate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SOPTopTemplates",
                columns: table => new
                {
                    TopTempId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    SOPAllowCode = table.Column<int>(nullable: false),
                    SOPAllowCodeLimit = table.Column<string>(nullable: true),
                    SOPCodePrefix = table.Column<string>(nullable: true),
                    SOPCodeSuffix = table.Column<string>(nullable: true),
                    SOPName = table.Column<string>(nullable: true),
                    SOPNameLimit = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOPTopTemplates", x => x.TopTempId);
                    table.ForeignKey(
                        name: "FK_SOPTopTemplates_TheCompanyInfo_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "TheCompanyInfo",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SOPTopTemplates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SOPTopTemplates_CompanyId",
                table: "SOPTopTemplates",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SOPTopTemplates_UserId",
                table: "SOPTopTemplates",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SOPTopTemplates");
        }
    }
}
