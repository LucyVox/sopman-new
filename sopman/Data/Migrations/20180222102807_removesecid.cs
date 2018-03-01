using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class removesecid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUpload_SOPSectionCreate_SectionId",
                table: "FileUpload");

            migrationBuilder.DropForeignKey(
                name: "FK_MultilineText_SOPSectionCreate_SectionId",
                table: "MultilineText");

            migrationBuilder.DropForeignKey(
                name: "FK_SingleLinkText_SOPSectionCreate_SectionId",
                table: "SingleLinkText");

            migrationBuilder.DropForeignKey(
                name: "FK_Table_SOPSectionCreate_SectionId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Table_SectionId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_SingleLinkText_SectionId",
                table: "SingleLinkText");

            migrationBuilder.DropIndex(
                name: "IX_MultilineText_SectionId",
                table: "MultilineText");

            migrationBuilder.DropIndex(
                name: "IX_FileUpload_SectionId",
                table: "FileUpload");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "SingleLinkText");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "MultilineText");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "FileUpload");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Table",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "SingleLinkText",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "MultilineText",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "FileUpload",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Table_SectionId",
                table: "Table",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleLinkText_SectionId",
                table: "SingleLinkText",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MultilineText_SectionId",
                table: "MultilineText",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUpload_SectionId",
                table: "FileUpload",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUpload_SOPSectionCreate_SectionId",
                table: "FileUpload",
                column: "SectionId",
                principalTable: "SOPSectionCreate",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MultilineText_SOPSectionCreate_SectionId",
                table: "MultilineText",
                column: "SectionId",
                principalTable: "SOPSectionCreate",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SingleLinkText_SOPSectionCreate_SectionId",
                table: "SingleLinkText",
                column: "SectionId",
                principalTable: "SOPSectionCreate",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Table_SOPSectionCreate_SectionId",
                table: "Table",
                column: "SectionId",
                principalTable: "SOPSectionCreate",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
