using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RACIResUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RACIInfUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RACIConUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RACIAccUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "RACIResUser");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RACIInfUser");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RACIConUser");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RACIAccUser");
        }
    }
}
