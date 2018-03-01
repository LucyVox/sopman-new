using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class soptempid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "soptoptempid",
                table: "SOPRACIRes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "soptoptempid",
                table: "SOPRACIInf",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "soptoptempid",
                table: "SOPRACICon",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "soptoptempid",
                table: "SOPRACIAcc",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "soptoptempid",
                table: "SOPRACIRes");

            migrationBuilder.DropColumn(
                name: "soptoptempid",
                table: "SOPRACIInf");

            migrationBuilder.DropColumn(
                name: "soptoptempid",
                table: "SOPRACICon");

            migrationBuilder.DropColumn(
                name: "soptoptempid",
                table: "SOPRACIAcc");
        }
    }
}
