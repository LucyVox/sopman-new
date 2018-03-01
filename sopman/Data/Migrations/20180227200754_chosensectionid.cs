using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class chosensectionid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "soptoptempid",
                table: "RACIResUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "soptoptempid",
                table: "RACIInfUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "soptoptempid",
                table: "RACIConUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "soptoptempid",
                table: "RACIAccUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "soptoptempid",
                table: "RACIResUser");

            migrationBuilder.DropColumn(
                name: "soptoptempid",
                table: "RACIInfUser");

            migrationBuilder.DropColumn(
                name: "soptoptempid",
                table: "RACIConUser");

            migrationBuilder.DropColumn(
                name: "soptoptempid",
                table: "RACIAccUser");
        }
    }
}
