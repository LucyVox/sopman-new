using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class SOPCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SOPSectionCreate",
                columns: table => new
                {
                    SectionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectionText = table.Column<string>(nullable: true),
                    TopTempId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOPSectionCreate", x => x.SectionId);
                    table.ForeignKey(
                        name: "FK_SOPSectionCreate_SOPTopTemplates_TopTempId",
                        column: x => x.TopTempId,
                        principalTable: "SOPTopTemplates",
                        principalColumn: "TopTempId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SOPSectionCreate_TopTempId",
                table: "SOPSectionCreate",
                column: "TopTempId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SOPSectionCreate");
        }
    }
}
