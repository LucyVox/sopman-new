using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class addarchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
