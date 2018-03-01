using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace sopman.Data.Migrations
{
    public partial class newraci : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SOPRACIAcc",
                columns: table => new
                {
                    RACIAccID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DepartmentId = table.Column<string>(nullable: true),
                    JobTitleId = table.Column<string>(nullable: true),
                    SOPTemplateProcessID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOPRACIAcc", x => x.RACIAccID);
                    table.ForeignKey(
                        name: "FK_SOPRACIAcc_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SOPRACIAcc_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "JobTitleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SOPRACICon",
                columns: table => new
                {
                    RACIConID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DepartmentId = table.Column<string>(nullable: true),
                    JobTitleId = table.Column<string>(nullable: true),
                    SOPTemplateProcessID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOPRACICon", x => x.RACIConID);
                    table.ForeignKey(
                        name: "FK_SOPRACICon_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SOPRACICon_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "JobTitleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SOPRACIInf",
                columns: table => new
                {
                    RACIInfID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DepartmentId = table.Column<string>(nullable: true),
                    JobTitleId = table.Column<string>(nullable: true),
                    SOPTemplateProcessID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOPRACIInf", x => x.RACIInfID);
                    table.ForeignKey(
                        name: "FK_SOPRACIInf_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SOPRACIInf_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "JobTitleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SOPRACIRes",
                columns: table => new
                {
                    RACIResID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DepartmentId = table.Column<string>(nullable: true),
                    JobTitleId = table.Column<string>(nullable: true),
                    SOPTemplateProcessID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOPRACIRes", x => x.RACIResID);
                    table.ForeignKey(
                        name: "FK_SOPRACIRes_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SOPRACIRes_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "JobTitleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SOPRACIAcc_DepartmentId",
                table: "SOPRACIAcc",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SOPRACIAcc_JobTitleId",
                table: "SOPRACIAcc",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_SOPRACICon_DepartmentId",
                table: "SOPRACICon",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SOPRACICon_JobTitleId",
                table: "SOPRACICon",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_SOPRACIInf_DepartmentId",
                table: "SOPRACIInf",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SOPRACIInf_JobTitleId",
                table: "SOPRACIInf",
                column: "JobTitleId");


            migrationBuilder.CreateIndex(
                name: "IX_SOPRACIRes_DepartmentId",
                table: "SOPRACIRes",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SOPRACIRes_JobTitleId",
                table: "SOPRACIRes",
                column: "JobTitleId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
