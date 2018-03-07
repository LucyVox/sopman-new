using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class DOTHECHANGES : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "RACIResUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "RACIInfUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "RACIConUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceId",
                table: "RACIAccUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "RACIResUser");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "RACIInfUser");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "RACIConUser");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "RACIAccUser");
        }
    }
}
