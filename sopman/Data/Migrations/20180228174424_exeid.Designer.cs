﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using sopman.Data;
using System;

namespace sopman.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180228174424_exeid")]
    partial class exeid
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+ClaimComp", b =>
                {
                    b.Property<int>("ClaimId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyId");

                    b.Property<string>("DepartmentId");

                    b.Property<string>("FirstName");

                    b.Property<string>("JobTitleId");

                    b.Property<string>("SecondName");

                    b.Property<string>("UserId");

                    b.HasKey("ClaimId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("JobTitleId");

                    b.HasIndex("UserId");

                    b.ToTable("CompanyClaim");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+CompanyInfo", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Logo");

                    b.Property<string>("Name");

                    b.Property<string>("SOPNumberFormat");

                    b.Property<string>("SOPStartNumber");

                    b.Property<string>("SettingTitle");

                    b.Property<string>("UserId");

                    b.HasKey("CompanyId");

                    b.HasIndex("UserId");

                    b.ToTable("TheCompanyInfo");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+DepartmentTable", b =>
                {
                    b.Property<string>("DepartmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyId");

                    b.Property<string>("DepartmentName");

                    b.HasKey("DepartmentId");

                    b.HasIndex("CompanyId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+EntMultilinkTextSec", b =>
                {
                    b.Property<int>("SubSecId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MultilineTextBlock");

                    b.Property<string>("NewTempId");

                    b.Property<string>("valuematch");

                    b.HasKey("SubSecId");

                    b.ToTable("UsedMultilineText");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+EntSingleLinkTextSec", b =>
                {
                    b.Property<int>("SubSecId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NewTempId");

                    b.Property<string>("SingleLinkTextBlock");

                    b.Property<string>("valuematch");

                    b.HasKey("SubSecId");

                    b.ToTable("UsedSingleLinkText");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+EntTableSec", b =>
                {
                    b.Property<int>("SubSecId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NewTempId");

                    b.Property<string>("TableHTML");

                    b.Property<string>("tableval");

                    b.Property<string>("valuematch");

                    b.HasKey("SubSecId");

                    b.ToTable("UsedTable");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+EntTableSecCols", b =>
                {
                    b.Property<int>("TableCls")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ColText");

                    b.Property<string>("NewTempId");

                    b.Property<string>("tableval");

                    b.HasKey("TableCls");

                    b.ToTable("UsedTableCols");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+EntTableSecRows", b =>
                {
                    b.Property<int>("TableRows")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NewTempId");

                    b.Property<string>("RowText");

                    b.Property<string>("tableval");

                    b.Property<string>("valuematch");

                    b.HasKey("TableRows");

                    b.ToTable("UsedTableRows");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+ExecuteSop", b =>
                {
                    b.Property<string>("ExecuteSopID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SectionId");

                    b.Property<string>("UserId");

                    b.HasKey("ExecuteSopID");

                    b.ToTable("ExecutedSop");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+FileUploadSec", b =>
                {
                    b.Property<int>("SubSecId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FileUrl");

                    b.Property<string>("valuematch");

                    b.HasKey("SubSecId");

                    b.ToTable("FileUpload");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+InstanceFiles", b =>
                {
                    b.Property<string>("InstanceFilesID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FileString");

                    b.Property<string>("InstanceId");

                    b.HasKey("InstanceFilesID");

                    b.ToTable("InstanceFile");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+JobTitlesTable", b =>
                {
                    b.Property<string>("JobTitleId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyId");

                    b.Property<string>("DepartmentId");

                    b.Property<string>("JobTitle");

                    b.Property<string>("TheDepartmentName");

                    b.HasKey("JobTitleId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("JobTitles");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+LiveSOP", b =>
                {
                    b.Property<string>("LiveSOPId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Id");

                    b.Property<string>("SOPTemplateID");

                    b.HasKey("LiveSOPId");

                    b.ToTable("LiveSOPs");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+MultilinkTextSec", b =>
                {
                    b.Property<int>("SubSecId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MultilineTextBlock");

                    b.Property<string>("valuematch");

                    b.HasKey("SubSecId");

                    b.ToTable("MultilineText");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+Project", b =>
                {
                    b.Property<string>("ProjectId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompId");

                    b.Property<string>("ProjectName");

                    b.HasKey("ProjectId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIAccChosenUser", b =>
                {
                    b.Property<int>("RACIAccChosenID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RACIAccID");

                    b.Property<string>("Status");

                    b.Property<string>("StatusComplete");

                    b.Property<int>("UserId");

                    b.Property<string>("soptoptempid");

                    b.HasKey("RACIAccChosenID");

                    b.ToTable("RACIAccUser");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIAccComp", b =>
                {
                    b.Property<int>("RACIAccCompID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceID");

                    b.Property<int>("RACIAccChosenID");

                    b.Property<string>("StatusComplete");

                    b.HasKey("RACIAccCompID");

                    b.ToTable("RACIAccComplete");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIAccount", b =>
                {
                    b.Property<int>("RACIAccID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DepartmentId");

                    b.Property<string>("JobTitleId");

                    b.Property<int>("SOPTemplateProcessID");

                    b.Property<string>("soptoptempid");

                    b.Property<string>("valuematch");

                    b.HasKey("RACIAccID");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("JobTitleId");

                    b.ToTable("SOPRACIAcc");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIAccRecu", b =>
                {
                    b.Property<int>("RACIAccRecusalID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceID");

                    b.Property<int>("RACIAccChosenID");

                    b.Property<string>("StatusRecusal");

                    b.HasKey("RACIAccRecusalID");

                    b.ToTable("RACIAccRecusal");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIConChosenUser", b =>
                {
                    b.Property<int>("RACIConChosenID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RACIConID");

                    b.Property<string>("Status");

                    b.Property<string>("StatusComplete");

                    b.Property<int>("UserId");

                    b.Property<string>("soptoptempid");

                    b.HasKey("RACIConChosenID");

                    b.ToTable("RACIConUser");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIConComp", b =>
                {
                    b.Property<int>("RACIConCompID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceID");

                    b.Property<int>("RACIConChosenID");

                    b.Property<string>("StatusComplete");

                    b.HasKey("RACIConCompID");

                    b.ToTable("RACIConComplete");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIConRecu", b =>
                {
                    b.Property<int>("RACIConRecusalID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceID");

                    b.Property<int>("RACIConChosenID");

                    b.Property<string>("StatusRecusal");

                    b.HasKey("RACIConRecusalID");

                    b.ToTable("RACIConRecusal");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIConsulted", b =>
                {
                    b.Property<int>("RACIConID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DepartmentId");

                    b.Property<string>("JobTitleId");

                    b.Property<int>("SOPTemplateProcessID");

                    b.Property<string>("soptoptempid");

                    b.Property<string>("valuematch");

                    b.HasKey("RACIConID");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("JobTitleId");

                    b.ToTable("SOPRACICon");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIInfChosenUser", b =>
                {
                    b.Property<int>("RACIInfChosenID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RACIInfID");

                    b.Property<string>("Status");

                    b.Property<string>("StatusComplete");

                    b.Property<int>("UserId");

                    b.Property<string>("soptoptempid");

                    b.HasKey("RACIInfChosenID");

                    b.ToTable("RACIInfUser");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIInfComp", b =>
                {
                    b.Property<int>("RACIInfCompID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceID");

                    b.Property<int>("RACIInfChosenID");

                    b.Property<string>("StatusComplete");

                    b.HasKey("RACIInfCompID");

                    b.ToTable("RACIInfComplete");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIInformed", b =>
                {
                    b.Property<int>("RACIInfID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DepartmentId");

                    b.Property<string>("JobTitleId");

                    b.Property<int>("SOPTemplateProcessID");

                    b.Property<string>("soptoptempid");

                    b.Property<string>("valuematch");

                    b.HasKey("RACIInfID");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("JobTitleId");

                    b.ToTable("SOPRACIInf");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIInfRecu", b =>
                {
                    b.Property<int>("RACIInfRecusalID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceID");

                    b.Property<int>("RACIInfChosenID");

                    b.Property<string>("StatusRecusal");

                    b.HasKey("RACIInfRecusalID");

                    b.ToTable("RACIInfRecusal");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIResChosenUser", b =>
                {
                    b.Property<int>("RACIResChosenID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RACIResID");

                    b.Property<string>("Status");

                    b.Property<string>("StatusComplete");

                    b.Property<int>("UserId");

                    b.Property<string>("soptoptempid");

                    b.HasKey("RACIResChosenID");

                    b.ToTable("RACIResUser");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIResComp", b =>
                {
                    b.Property<int>("RACIResCompID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceID");

                    b.Property<int>("RACIResChosenID");

                    b.Property<string>("StatusComplete");

                    b.HasKey("RACIResCompID");

                    b.ToTable("RACIResComplete");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIResposible", b =>
                {
                    b.Property<int>("RACIResID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DepartmentId");

                    b.Property<string>("JobTitleId");

                    b.Property<int>("SOPTemplateProcessID");

                    b.Property<string>("soptoptempid");

                    b.Property<string>("valuematch");

                    b.HasKey("RACIResID");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("JobTitleId");

                    b.ToTable("SOPRACIRes");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIResRecu", b =>
                {
                    b.Property<int>("RACIResRecusalID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceID");

                    b.Property<int>("RACIResChosenID");

                    b.Property<string>("StatusRecusal");

                    b.HasKey("RACIResRecusalID");

                    b.ToTable("RACIResRecusal");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SecOrder", b =>
                {
                    b.Property<int>("SecOrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SubSecId");

                    b.HasKey("SecOrderId");

                    b.ToTable("SectionOrder");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SingleLinkTextSec", b =>
                {
                    b.Property<int>("SubSecId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SingleLinkTextBlock");

                    b.Property<string>("UserId");

                    b.Property<string>("valuematch");

                    b.HasKey("SubSecId");

                    b.HasIndex("UserId");

                    b.ToTable("SingleLinkText");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPInstanceProcess", b =>
                {
                    b.Property<int>("SOPInstancesProcessId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DueDate");

                    b.Property<string>("ExternalDocument");

                    b.Property<string>("SOPTemplateID");

                    b.Property<string>("valuematch");

                    b.HasKey("SOPInstancesProcessId");

                    b.ToTable("SOPInstanceProcesses");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPInstanceProcessFiles", b =>
                {
                    b.Property<int>("SOPInstanceProcessFilesId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SOPInstancesProcessId");

                    b.Property<string>("filepath");

                    b.HasKey("SOPInstanceProcessFilesId");

                    b.ToTable("SOPInstanceProcessesFiles");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPNewInstance", b =>
                {
                    b.Property<string>("InstanceId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstanceExpire");

                    b.Property<string>("InstanceRef");

                    b.Property<string>("ProjectId");

                    b.Property<string>("SOPTemplateID");

                    b.HasKey("InstanceId");

                    b.ToTable("NewInstance");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPProcessFiles", b =>
                {
                    b.Property<int>("FileID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FilesString");

                    b.Property<string>("SOPTemplateProcessID");

                    b.HasKey("FileID");

                    b.ToTable("ProcessFiles");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPSectionCreator", b =>
                {
                    b.Property<int>("SectionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SectionText");

                    b.Property<int>("TopTempId");

                    b.Property<string>("valuematch");

                    b.HasKey("SectionId");

                    b.HasIndex("TopTempId");

                    b.ToTable("SOPSectionCreate");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPTemplate", b =>
                {
                    b.Property<string>("SOPTemplateID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ExpireDate");

                    b.Property<string>("SOPCode");

                    b.Property<string>("TempName");

                    b.Property<int>("TopTempId");

                    b.HasKey("SOPTemplateID");

                    b.HasIndex("TopTempId");

                    b.ToTable("SOPNewTemplate");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPTemplateProcess", b =>
                {
                    b.Property<string>("SOPTemplateProcessID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ProcessCount");

                    b.Property<string>("ProcessDesc");

                    b.Property<string>("ProcessName");

                    b.Property<string>("ProcessType");

                    b.Property<string>("SOPTemplateID");

                    b.Property<string>("valuematch");

                    b.HasKey("SOPTemplateProcessID");

                    b.HasIndex("SOPTemplateID");

                    b.ToTable("SOPProcess");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPTemplateProcesses", b =>
                {
                    b.Property<int>("SOPTemplateProcessID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ProcessCount");

                    b.Property<string>("ProcessDesc");

                    b.Property<string>("ProcessName");

                    b.Property<string>("ProcessType");

                    b.Property<string>("SOPTemplateID");

                    b.Property<string>("valuematch");

                    b.HasKey("SOPTemplateProcessID");

                    b.HasIndex("SOPTemplateID");

                    b.ToTable("SOPProcessTempls");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPTopTemplate", b =>
                {
                    b.Property<int>("TopTempId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyId");

                    b.Property<int>("SOPAllowCode");

                    b.Property<string>("SOPAllowCodeLimit");

                    b.Property<string>("SOPCodePrefix");

                    b.Property<string>("SOPCodeSuffix");

                    b.Property<string>("SOPName");

                    b.Property<int>("SOPNameLimit");

                    b.Property<int>("SOPNameLimitTF");

                    b.Property<string>("UserId");

                    b.HasKey("TopTempId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("UserId");

                    b.ToTable("SOPTopTemplates");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+TableSec", b =>
                {
                    b.Property<int>("SubSecId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("TableHTML");

                    b.Property<string>("valuematch");

                    b.HasKey("SubSecId");

                    b.ToTable("Table");
                });

            modelBuilder.Entity("sopman.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("sopman.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("sopman.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("sopman.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("sopman.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+ClaimComp", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+CompanyInfo", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("sopman.Data.ApplicationDbContext+DepartmentTable", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("sopman.Data.ApplicationDbContext+JobTitlesTable", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");

                    b.HasOne("sopman.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+CompanyInfo", b =>
                {
                    b.HasOne("sopman.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+DepartmentTable", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+CompanyInfo", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+JobTitlesTable", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+CompanyInfo", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("sopman.Data.ApplicationDbContext+DepartmentTable", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIAccount", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+DepartmentTable", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("sopman.Data.ApplicationDbContext+JobTitlesTable", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIConsulted", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+DepartmentTable", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("sopman.Data.ApplicationDbContext+JobTitlesTable", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIInformed", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+DepartmentTable", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("sopman.Data.ApplicationDbContext+JobTitlesTable", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+RACIResposible", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+DepartmentTable", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("sopman.Data.ApplicationDbContext+JobTitlesTable", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SingleLinkTextSec", b =>
                {
                    b.HasOne("sopman.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPSectionCreator", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+SOPTopTemplate", "SOPTopTemplate")
                        .WithMany()
                        .HasForeignKey("TopTempId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPTemplate", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+SOPTopTemplate", "SOPTopTemplate")
                        .WithMany()
                        .HasForeignKey("TopTempId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPTemplateProcess", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+SOPTemplate", "SOPNewTemplate")
                        .WithMany()
                        .HasForeignKey("SOPTemplateID");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPTemplateProcesses", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+SOPTemplate", "SOPNewTemplate")
                        .WithMany()
                        .HasForeignKey("SOPTemplateID");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPTopTemplate", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+CompanyInfo", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("sopman.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}