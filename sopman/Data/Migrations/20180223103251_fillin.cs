using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class fillin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsedMultilineText",
                columns: table => new
                {
                    SubSecId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MultilineTextBlock = table.Column<string>(nullable: true),
                    valuematch = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedMultilineText", x => x.SubSecId);
                });

            migrationBuilder.CreateTable(
                name: "UsedSingleLinkText",
                columns: table => new
                {
                    SubSecId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SingleLinkTextBlock = table.Column<string>(nullable: true),
                    valuematch = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedSingleLinkText", x => x.SubSecId);
                });

            migrationBuilder.CreateTable(
                name: "UsedTable",
                columns: table => new
                {
                    SubSecId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TableHTML = table.Column<string>(nullable: true),
                    tableval = table.Column<string>(nullable: true),
                    valuematch = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedTable", x => x.SubSecId);
                });

            migrationBuilder.CreateTable(
                name: "UsedTableCols",
                columns: table => new
                {
                    TableCls = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ColText = table.Column<string>(nullable: true),
                    tableval = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedTableCols", x => x.TableCls);
                });

            migrationBuilder.CreateTable(
                name: "UsedTableRows",
                columns: table => new
                {
                    TableRows = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowText = table.Column<string>(nullable: true),
                    tableval = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedTableRows", x => x.TableRows);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedMultilineText");

            migrationBuilder.DropTable(
                name: "UsedSingleLinkText");

            migrationBuilder.DropTable(
                name: "UsedTable");

            migrationBuilder.DropTable(
                name: "UsedTableCols");

            migrationBuilder.DropTable(
                name: "UsedTableRows");
        }
    }
}
