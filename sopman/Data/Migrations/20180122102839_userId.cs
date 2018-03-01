using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class userId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SingleLinkText",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SingleLinkText_UserId",
                table: "SingleLinkText",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SingleLinkText_AspNetUsers_UserId",
                table: "SingleLinkText",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SingleLinkText_AspNetUsers_UserId",
                table: "SingleLinkText");

            migrationBuilder.DropIndex(
                name: "IX_SingleLinkText_UserId",
                table: "SingleLinkText");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SingleLinkText");
        }
    }
}
