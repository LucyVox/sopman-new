using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class compupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "CompanyClaim",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "CompanyClaim",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitleId",
                table: "CompanyClaim",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "CompanyClaim",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyClaim_DepartmentId",
                table: "CompanyClaim",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyClaim_JobTitleId",
                table: "CompanyClaim",
                column: "JobTitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyClaim_Departments_DepartmentId",
                table: "CompanyClaim",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyClaim_JobTitles_JobTitleId",
                table: "CompanyClaim",
                column: "JobTitleId",
                principalTable: "JobTitles",
                principalColumn: "JobTitleId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyClaim_Departments_DepartmentId",
                table: "CompanyClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyClaim_JobTitles_JobTitleId",
                table: "CompanyClaim");

            migrationBuilder.DropIndex(
                name: "IX_CompanyClaim_DepartmentId",
                table: "CompanyClaim");

            migrationBuilder.DropIndex(
                name: "IX_CompanyClaim_JobTitleId",
                table: "CompanyClaim");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "CompanyClaim");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "CompanyClaim");

            migrationBuilder.DropColumn(
                name: "JobTitleId",
                table: "CompanyClaim");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "CompanyClaim");
        }
    }
}
