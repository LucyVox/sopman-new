using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class updatedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOPProcess_SOPNewTemplate_TopTempId",
                table: "SOPProcess");

            migrationBuilder.DropIndex(
                name: "IX_SOPProcess_TopTempId",
                table: "SOPProcess");

            migrationBuilder.DropColumn(
                name: "TopTempId",
                table: "SOPProcess");

            migrationBuilder.AlterColumn<string>(
                name: "SOPTemplateID",
                table: "SOPProcess",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SOPProcess_SOPTemplateID",
                table: "SOPProcess",
                column: "SOPTemplateID");

            migrationBuilder.AddForeignKey(
                name: "FK_SOPProcess_SOPNewTemplate_SOPTemplateID",
                table: "SOPProcess",
                column: "SOPTemplateID",
                principalTable: "SOPNewTemplate",
                principalColumn: "SOPTemplateID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOPProcess_SOPNewTemplate_SOPTemplateID",
                table: "SOPProcess");

            migrationBuilder.DropIndex(
                name: "IX_SOPProcess_SOPTemplateID",
                table: "SOPProcess");

            migrationBuilder.AlterColumn<string>(
                name: "SOPTemplateID",
                table: "SOPProcess",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopTempId",
                table: "SOPProcess",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SOPProcess_TopTempId",
                table: "SOPProcess",
                column: "TopTempId");

            migrationBuilder.AddForeignKey(
                name: "FK_SOPProcess_SOPNewTemplate_TopTempId",
                table: "SOPProcess",
                column: "TopTempId",
                principalTable: "SOPNewTemplate",
                principalColumn: "SOPTemplateID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
