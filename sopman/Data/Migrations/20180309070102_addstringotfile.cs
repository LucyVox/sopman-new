using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class addstringotfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "SOPInstanceProcessesFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "SOPInstanceProcessesFiles");
        }
    }
}
