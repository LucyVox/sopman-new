using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class Sections1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MultilineText",
                columns: table => new
                {
                    MultilineTextID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MultilineTextBlock = table.Column<string>(nullable: true),
                    SectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultilineText", x => x.MultilineTextID);
                    table.ForeignKey(
                        name: "FK_MultilineText_SOPSectionCreate_SectionId",
                        column: x => x.SectionId,
                        principalTable: "SOPSectionCreate",
                        principalColumn: "SectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleLinkText",
                columns: table => new
                {
                    SingleLinkTextID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectionId = table.Column<int>(nullable: false),
                    SingleLinkTextBlock = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleLinkText", x => x.SingleLinkTextID);
                    table.ForeignKey(
                        name: "FK_SingleLinkText_SOPSectionCreate_SectionId",
                        column: x => x.SectionId,
                        principalTable: "SOPSectionCreate",
                        principalColumn: "SectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultilineText_SectionId",
                table: "MultilineText",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleLinkText_SectionId",
                table: "SingleLinkText",
                column: "SectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultilineText");

            migrationBuilder.DropTable(
                name: "SingleLinkText");
        }
    }
}
