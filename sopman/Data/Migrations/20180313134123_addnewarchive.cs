using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class addnewarchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Archived",
                table: "Archived");

            migrationBuilder.RenameTable(
                name: "Archived",
                newName: "ArchivedInstacned");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArchivedInstacned",
                table: "ArchivedInstacned",
                column: "ArchiveId");

            migrationBuilder.CreateTable(
                name: "ArchiveASOP",
                columns: table => new
                {
                    ArchiveId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SOPTemplateId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveASOP", x => x.ArchiveId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchiveASOP");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArchivedInstacned",
                table: "ArchivedInstacned");

            migrationBuilder.RenameTable(
                name: "ArchivedInstacned",
                newName: "Archived");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Archived",
                table: "Archived",
                column: "ArchiveId");
        }
    }
}
