using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class apparoveraddstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "SOPNewTemplate");

            migrationBuilder.AddColumn<string>(
                name: "ApproverStatus",
                table: "InstanceApprovers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproverStatus",
                table: "InstanceApprovers");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "SOPNewTemplate",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
