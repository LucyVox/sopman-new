using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class MigrateUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableSecID",
                table: "Table",
                newName: "SubSecId");

            migrationBuilder.RenameColumn(
                name: "SingleLinkTextID",
                table: "SingleLinkText",
                newName: "SubSecId");

            migrationBuilder.RenameColumn(
                name: "MultilineTextID",
                table: "MultilineText",
                newName: "SubSecId");

            migrationBuilder.RenameColumn(
                name: "FileUploadId",
                table: "FileUpload",
                newName: "SubSecId");

            migrationBuilder.CreateTable(
                name: "SectionOrder",
                columns: table => new
                {
                    SecOrderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileUploadSubSecId = table.Column<int>(nullable: true),
                    MultilineTextSubSecId = table.Column<int>(nullable: true),
                    SubSecId = table.Column<int>(nullable: true),
                    TableSubSecId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionOrder", x => x.SecOrderId);
                    table.ForeignKey(
                        name: "FK_SectionOrder_FileUpload_FileUploadSubSecId",
                        column: x => x.FileUploadSubSecId,
                        principalTable: "FileUpload",
                        principalColumn: "SubSecId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectionOrder_MultilineText_MultilineTextSubSecId",
                        column: x => x.MultilineTextSubSecId,
                        principalTable: "MultilineText",
                        principalColumn: "SubSecId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectionOrder_SingleLinkText_SubSecId",
                        column: x => x.SubSecId,
                        principalTable: "SingleLinkText",
                        principalColumn: "SubSecId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectionOrder_Table_TableSubSecId",
                        column: x => x.TableSubSecId,
                        principalTable: "Table",
                        principalColumn: "SubSecId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SectionOrder_FileUploadSubSecId",
                table: "SectionOrder",
                column: "FileUploadSubSecId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionOrder_MultilineTextSubSecId",
                table: "SectionOrder",
                column: "MultilineTextSubSecId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionOrder_SubSecId",
                table: "SectionOrder",
                column: "SubSecId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionOrder_TableSubSecId",
                table: "SectionOrder",
                column: "TableSubSecId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectionOrder");

            migrationBuilder.RenameColumn(
                name: "SubSecId",
                table: "Table",
                newName: "TableSecID");

            migrationBuilder.RenameColumn(
                name: "SubSecId",
                table: "SingleLinkText",
                newName: "SingleLinkTextID");

            migrationBuilder.RenameColumn(
                name: "SubSecId",
                table: "MultilineText",
                newName: "MultilineTextID");

            migrationBuilder.RenameColumn(
                name: "SubSecId",
                table: "FileUpload",
                newName: "FileUploadId");
        }
    }
}
