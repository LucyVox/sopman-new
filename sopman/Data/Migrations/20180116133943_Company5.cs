using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class Company5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TheCompanyInfo_AspNetUsers_UserId",
                table: "TheCompanyInfo");

            migrationBuilder.DropIndex(
                name: "IX_TheCompanyInfo_UserId",
                table: "TheCompanyInfo");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TheCompanyInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TheCompanyInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TheCompanyInfo_UserId",
                table: "TheCompanyInfo",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TheCompanyInfo_AspNetUsers_UserId",
                table: "TheCompanyInfo",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
