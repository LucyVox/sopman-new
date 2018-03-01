using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class addvalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SOPTemplateProcessID",
                table: "SOPRACIInf",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "SOPRACIInf",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SOPTemplateProcessID",
                table: "SOPRACICon",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "SOPRACICon",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SOPTemplateProcessID",
                table: "SOPRACIAcc",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "valuematch",
                table: "SOPRACIAcc",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "SOPRACIInf");

            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "SOPRACICon");

            migrationBuilder.DropColumn(
                name: "valuematch",
                table: "SOPRACIAcc");

            migrationBuilder.AlterColumn<string>(
                name: "SOPTemplateProcessID",
                table: "SOPRACIInf",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "SOPTemplateProcessID",
                table: "SOPRACICon",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "SOPTemplateProcessID",
                table: "SOPRACIAcc",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
