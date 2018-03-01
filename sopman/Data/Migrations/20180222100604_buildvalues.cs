using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class buildvalues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "Table",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "SOPSectionCreate",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "SingleLinkText",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "MultilineText",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "FileUpload",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "SOPSectionCreate");

            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "SingleLinkText");

            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "MultilineText");

            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "FileUpload");
        }
    }
}
