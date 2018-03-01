using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class constatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RACIAccComplete",
                columns: table => new
                {
                    RACIAccCompID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIAccChosenID = table.Column<int>(nullable: false),
                    StatusComplete = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIAccComplete", x => x.RACIAccCompID);
                });

            migrationBuilder.CreateTable(
                name: "RACIAccRecusal",
                columns: table => new
                {
                    RACIAccRecusalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIAccChosenID = table.Column<int>(nullable: false),
                    StatusRecusal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIAccRecusal", x => x.RACIAccRecusalID);
                });

            migrationBuilder.CreateTable(
                name: "RACIConComplete",
                columns: table => new
                {
                    RACIConCompID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIConChosenID = table.Column<int>(nullable: false),
                    StatusComplete = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIConComplete", x => x.RACIConCompID);
                });

            migrationBuilder.CreateTable(
                name: "RACIConRecusal",
                columns: table => new
                {
                    RACIConRecusalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIConChosenID = table.Column<int>(nullable: false),
                    StatusRecusal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIConRecusal", x => x.RACIConRecusalID);
                });

            migrationBuilder.CreateTable(
                name: "RACIInfComplete",
                columns: table => new
                {
                    RACIInfCompID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIInfChosenID = table.Column<int>(nullable: false),
                    StatusComplete = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIInfComplete", x => x.RACIInfCompID);
                });

            migrationBuilder.CreateTable(
                name: "RACIInfRecusal",
                columns: table => new
                {
                    RACIInfRecusalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIInfChosenID = table.Column<int>(nullable: false),
                    StatusRecusal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIInfRecusal", x => x.RACIInfRecusalID);
                });

            migrationBuilder.CreateTable(
                name: "RACIResComplete",
                columns: table => new
                {
                    RACIResCompID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIResChosenID = table.Column<int>(nullable: false),
                    StatusComplete = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIResComplete", x => x.RACIResCompID);
                });

            migrationBuilder.CreateTable(
                name: "RACIResRecusal",
                columns: table => new
                {
                    RACIResRecusalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RACIResChosenID = table.Column<int>(nullable: false),
                    StatusRecusal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACIResRecusal", x => x.RACIResRecusalID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RACIAccComplete");

            migrationBuilder.DropTable(
                name: "RACIAccRecusal");

            migrationBuilder.DropTable(
                name: "RACIConComplete");

            migrationBuilder.DropTable(
                name: "RACIConRecusal");

            migrationBuilder.DropTable(
                name: "RACIInfComplete");

            migrationBuilder.DropTable(
                name: "RACIInfRecusal");

            migrationBuilder.DropTable(
                name: "RACIResComplete");

            migrationBuilder.DropTable(
                name: "RACIResRecusal");
        }
    }
}
