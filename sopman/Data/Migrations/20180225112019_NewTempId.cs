using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class NewTempId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewTempId",
                table: "UsedTableRows",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewTempId",
                table: "UsedTableCols",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewTempId",
                table: "UsedTable",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewTempId",
                table: "UsedSingleLinkText",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewTempId",
                table: "UsedMultilineText",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewTempId",
                table: "UsedTableRows");

            migrationBuilder.DropColumn(
                name: "NewTempId",
                table: "UsedTableCols");

            migrationBuilder.DropColumn(
                name: "NewTempId",
                table: "UsedTable");

            migrationBuilder.DropColumn(
                name: "NewTempId",
                table: "UsedSingleLinkText");

            migrationBuilder.DropColumn(
                name: "NewTempId",
                table: "UsedMultilineText");
        }
    }
}
