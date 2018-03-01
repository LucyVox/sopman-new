using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class changestatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusComplete",
                table: "RACIResUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusComplete",
                table: "RACIInfUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusComplete",
                table: "RACIConUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusComplete",
                table: "RACIAccUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusComplete",
                table: "RACIResUser");

            migrationBuilder.DropColumn(
                name: "StatusComplete",
                table: "RACIInfUser");

            migrationBuilder.DropColumn(
                name: "StatusComplete",
                table: "RACIConUser");

            migrationBuilder.DropColumn(
                name: "StatusComplete",
                table: "RACIAccUser");
        }
    }
}
