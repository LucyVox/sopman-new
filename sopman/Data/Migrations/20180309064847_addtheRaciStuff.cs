using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class addtheRaciStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "SOPRACIInf",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SOPRACIInf",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SOPRACIInf",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "SOPRACICon",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SOPRACICon",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SOPRACICon",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "SOPRACIAcc",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SOPRACIAcc",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SOPRACIAcc",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "SOPRACIInf");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SOPRACIInf");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SOPRACIInf");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "SOPRACICon");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SOPRACICon");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SOPRACICon");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "SOPRACIAcc");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SOPRACIAcc");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SOPRACIAcc");
        }
    }
}
