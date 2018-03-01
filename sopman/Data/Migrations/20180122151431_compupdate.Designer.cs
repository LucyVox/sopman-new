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
    [Migration("20180122151431_compupdate")]
    partial class compupdate
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

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+FileUploadSec", b =>
                {
                    b.Property<int>("FileUploadId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FileUrl");

                    b.Property<int>("SectionId");

                    b.HasKey("FileUploadId");

                    b.HasIndex("SectionId");

                    b.ToTable("FileUpload");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+JobTitlesTable", b =>
                {
                    b.Property<string>("JobTitleId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyId");

                    b.Property<string>("DepartmentId");

                    b.Property<string>("JobTitle");

                    b.HasKey("JobTitleId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("JobTitles");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+MultilinkTextSec", b =>
                {
                    b.Property<int>("MultilineTextID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MultilineTextBlock");

                    b.Property<int>("SectionId");

                    b.HasKey("MultilineTextID");

                    b.HasIndex("SectionId");

                    b.ToTable("MultilineText");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SingleLinkTextSec", b =>
                {
                    b.Property<int>("SingleLinkTextID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SectionId");

                    b.Property<string>("SingleLinkTextBlock");

                    b.Property<string>("UserId");

                    b.HasKey("SingleLinkTextID");

                    b.HasIndex("SectionId");

                    b.HasIndex("UserId");

                    b.ToTable("SingleLinkText");
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SOPSectionCreator", b =>
                {
                    b.Property<int>("SectionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SectionText");

                    b.Property<int>("TopTempId");

                    b.HasKey("SectionId");

                    b.HasIndex("TopTempId");

                    b.ToTable("SOPSectionCreate");
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
                    b.Property<int>("TableSecID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SectionId");

                    b.Property<string>("TableHTML");

                    b.HasKey("TableSecID");

                    b.HasIndex("SectionId");

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

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+FileUploadSec", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+SOPSectionCreator", "SOPSectionCreate")
                        .WithMany()
                        .HasForeignKey("SectionId")
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

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+MultilinkTextSec", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+SOPSectionCreator", "SOPSectionCreate")
                        .WithMany()
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+SingleLinkTextSec", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+SOPSectionCreator", "SOPSectionCreate")
                        .WithMany()
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity("sopman.Data.ApplicationDbContext+TableSec", b =>
                {
                    b.HasOne("sopman.Data.ApplicationDbContext+SOPSectionCreator", "SOPSectionCreate")
                        .WithMany()
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
