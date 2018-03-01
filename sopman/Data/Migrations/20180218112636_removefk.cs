using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class removefk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessFiles_SOPProcess_TopTempId",
                table: "ProcessFiles");

            migrationBuilder.DropIndex(
                name: "IX_ProcessFiles_TopTempId",
                table: "ProcessFiles");

            migrationBuilder.DropColumn(
                name: "TopTempId",
                table: "ProcessFiles");
                
            migrationBuilder.DropColumn(
                name: "SOPTemplateProcessID",
                table: "ProcessFiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
