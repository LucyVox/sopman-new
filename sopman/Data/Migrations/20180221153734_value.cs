using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class value : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "SOPRACIRes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "SOPProcess",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SOPTemplateProcessID",
                table: "ProcessFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "SOPRACIRes");

            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "SOPProcess");

            migrationBuilder.DropColumn(
                name: "SOPTemplateProcessID",
                table: "ProcessFiles");
        }
    }
}
