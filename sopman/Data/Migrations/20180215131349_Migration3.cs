using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class Migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionOrder_FileUpload_FileUploadSubSecId",
                table: "SectionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionOrder_MultilineText_MultilineTextSubSecId",
                table: "SectionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionOrder_SingleLinkText_SubSecId",
                table: "SectionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionOrder_Table_TableSubSecId",
                table: "SectionOrder");

            migrationBuilder.DropIndex(
                name: "IX_SectionOrder_FileUploadSubSecId",
                table: "SectionOrder");

            migrationBuilder.DropIndex(
                name: "IX_SectionOrder_MultilineTextSubSecId",
                table: "SectionOrder");

            migrationBuilder.DropIndex(
                name: "IX_SectionOrder_SubSecId",
                table: "SectionOrder");

            migrationBuilder.DropIndex(
                name: "IX_SectionOrder_TableSubSecId",
                table: "SectionOrder");

            migrationBuilder.DropColumn(
                name: "FileUploadSubSecId",
                table: "SectionOrder");

            migrationBuilder.DropColumn(
                name: "MultilineTextSubSecId",
                table: "SectionOrder");

            migrationBuilder.DropColumn(
                name: "TableSubSecId",
                table: "SectionOrder");

            migrationBuilder.AlterColumn<int>(
                name: "SubSecId",
                table: "SectionOrder",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SubSecId",
                table: "SectionOrder",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "FileUploadSubSecId",
                table: "SectionOrder",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MultilineTextSubSecId",
                table: "SectionOrder",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TableSubSecId",
                table: "SectionOrder",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_SectionOrder_FileUpload_FileUploadSubSecId",
                table: "SectionOrder",
                column: "FileUploadSubSecId",
                principalTable: "FileUpload",
                principalColumn: "SubSecId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SectionOrder_MultilineText_MultilineTextSubSecId",
                table: "SectionOrder",
                column: "MultilineTextSubSecId",
                principalTable: "MultilineText",
                principalColumn: "SubSecId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SectionOrder_SingleLinkText_SubSecId",
                table: "SectionOrder",
                column: "SubSecId",
                principalTable: "SingleLinkText",
                principalColumn: "SubSecId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SectionOrder_Table_TableSubSecId",
                table: "SectionOrder",
                column: "TableSubSecId",
                principalTable: "Table",
                principalColumn: "SubSecId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
