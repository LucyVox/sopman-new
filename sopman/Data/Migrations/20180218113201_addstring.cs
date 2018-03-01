using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class addstring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropColumn(
                name: "SOPTemplateProcessID",
                table: "ProcessFiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
