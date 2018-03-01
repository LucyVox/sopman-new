using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class racichosen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RACIAccUser",
                columns: table => new
                {
                    RACIAccChosenID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIAccID = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIAccUser", x => x.RACIAccChosenID);
                });

            migrationBuilder.CreateTable(
                name: "RACIConUser",
                columns: table => new
                {
                    RACIConChosenID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIConID = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIConUser", x => x.RACIConChosenID);
                });

            migrationBuilder.CreateTable(
                name: "RACIInfUser",
                columns: table => new
                {
                    RACIInfChosenID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIInfID = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIInfUser", x => x.RACIInfChosenID);
                });

            migrationBuilder.CreateTable(
                name: "RACIResUser",
                columns: table => new
                {
                    RACIResChosenID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIResID = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIResUser", x => x.RACIResChosenID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RACIAccUser");

            migrationBuilder.DropTable(
                name: "RACIConUser");

            migrationBuilder.DropTable(
                name: "RACIInfUser");

            migrationBuilder.DropTable(
                name: "RACIResUser");
        }
    }
}
