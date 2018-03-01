using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class newinstance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewInstance",
                columns: table => new
                {
                    InstanceId = table.Column<string>(nullable: false),
                    InstanceExpire = table.Column<string>(nullable: true),
                    InstanceRef = table.Column<string>(nullable: true),
                    ProjectId = table.Column<string>(nullable: true),
                    SOPTemplateID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewInstance", x => x.InstanceId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
