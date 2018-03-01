using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class SOPMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SOPNewTemplate",
                columns: table => new
                {
                    SOPTemplateID = table.Column<string>(nullable: false),
                    SOPCode = table.Column<string>(nullable: true),
                    TempName = table.Column<string>(nullable: true),
                    TopTempId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOPNewTemplate", x => x.SOPTemplateID);
                    table.ForeignKey(
                        name: "FK_SOPNewTemplate_SOPTopTemplates_TopTempId",
                        column: x => x.TopTempId,
                        principalTable: "SOPTopTemplates",
                        principalColumn: "TopTempId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SOPNewTemplate_TopTempId",
                table: "SOPNewTemplate",
                column: "TopTempId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SOPNewTemplate");
        }
    }
}
