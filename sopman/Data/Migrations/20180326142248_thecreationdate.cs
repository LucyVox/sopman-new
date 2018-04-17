using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class thecreationdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TheCreateDae",
                table: "SOPNewTemplate",
                nullable: false,
                type: "datetime2",
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TheCreateDae",
                table: "SOPNewTemplate");
        }
    }
}
