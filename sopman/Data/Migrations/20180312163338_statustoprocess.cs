using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class statustoprocess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "processStatus",
                table: "SOPProcessTempls",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "processStatus",
                table: "SOPProcessTempls");
        }
    }
}
