using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class exeid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstanceID",
                table: "RACIResRecusal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceID",
                table: "RACIResComplete",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceID",
                table: "RACIInfRecusal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceID",
                table: "RACIInfComplete",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceID",
                table: "RACIConRecusal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceID",
                table: "RACIConComplete",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceID",
                table: "RACIAccRecusal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceID",
                table: "RACIAccComplete",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstanceID",
                table: "RACIResRecusal");

            migrationBuilder.DropColumn(
                name: "InstanceID",
                table: "RACIResComplete");

            migrationBuilder.DropColumn(
                name: "InstanceID",
                table: "RACIInfRecusal");

            migrationBuilder.DropColumn(
                name: "InstanceID",
                table: "RACIInfComplete");

            migrationBuilder.DropColumn(
                name: "InstanceID",
                table: "RACIConRecusal");

            migrationBuilder.DropColumn(
                name: "InstanceID",
                table: "RACIConComplete");

            migrationBuilder.DropColumn(
                name: "InstanceID",
                table: "RACIAccRecusal");

            migrationBuilder.DropColumn(
                name: "InstanceID",
                table: "RACIAccComplete");
        }
    }
}
