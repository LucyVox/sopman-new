using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class sopprocesses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SOPProcessTempls",
                columns: table => new
                {
                    SOPTemplateProcessID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProcessCount = table.Column<int>(nullable: false),
                    ProcessDesc = table.Column<string>(nullable: true),
                    ProcessName = table.Column<string>(nullable: true),
                    ProcessType = table.Column<string>(nullable: true),
                    SOPTemplateID = table.Column<string>(nullable: true),
                    valuematch = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOPProcessTempls", x => x.SOPTemplateProcessID);
                    table.ForeignKey(
                        name: "FK_SOPProcessTempls_SOPNewTemplate_SOPTemplateID",
                        column: x => x.SOPTemplateID,
                        principalTable: "SOPNewTemplate",
                        principalColumn: "SOPTemplateID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SOPProcessTempls_SOPTemplateID",
                table: "SOPProcessTempls",
                column: "SOPTemplateID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SOPProcessTempls");
        }
    }
}
