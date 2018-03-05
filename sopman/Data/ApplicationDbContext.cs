using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using sopman.Models;

namespace sopman.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public class CompanyInfo
        {
            [Key]
            public int CompanyId { get; set; }
            public string Name { get; set; }

           [DataType(DataType.ImageUrl)]
            public string Logo { get; set; }
            //public HttpPostedFileBase Logo { get; set; }

            public string SOPNumberFormat { get; set; }
            public string SOPStartNumber { get; set; }
            public string SettingTitle { get; set; }
            public string UserId { get; set; }
            [ForeignKey("UserId")]
            public virtual ApplicationUser ApplicationUser { get; set; }
        }

        public DbSet<CompanyInfo> TheCompanyInfo { get; set; }

        public class SOPTopTemplate
        {
            [Key]
            public int TopTempId { get; set; }

            public int CompanyId { get; set; }
            [ForeignKey("CompanyId")]
            public virtual CompanyInfo Company { get; set; }

            public string UserId { get; set; }
            [ForeignKey("UserId")]
            public virtual ApplicationUser ApplicationUser { get; set; }

            public string SOPName { get; set; }
            public int SOPNameLimitTF { get; set; }
            public int SOPNameLimit { get; set; }
            public string SOPCodePrefix { get; set; }
            public string SOPCodeSuffix { get; set; }
            public int SOPAllowCode { get; set; }
            public string SOPAllowCodeLimit { get; set; }

        }

        public DbSet<SOPTopTemplate> SOPTopTemplates { get; set; }

        public class ClaimComp
        {
            [Key]
            public int ClaimId { get; set; }

            public int CompanyId { get; set; }
            [ForeignKey("CompanyId")]
            public virtual CompanyInfo Company { get; set; }

            public string UserId { get; set; }
            [ForeignKey("UserId")]
            public virtual ApplicationUser ApplicationUser { get; set; }

            public string FirstName { get; set; }

            public string SecondName { get; set; }

            public string DepartmentId { get; set; }
            [ForeignKey("DepartmentId")]
            public virtual DepartmentTable Department { get; set; }

            public string JobTitleId { get; set; }
            [ForeignKey("JobTitleId")]
            public virtual JobTitlesTable JobTitle { get; set; }

        }

        public DbSet<ClaimComp> CompanyClaim { get; set; }

        public class SOPSectionCreator
        { 
            [Key]
            public int SectionId { get; set; }

            public int TopTempId { get; set; }
            [ForeignKey("TopTempId")]
            public virtual SOPTopTemplate SOPTopTemplate { get; set; }

            public string SectionText { get; set; }
            public string valuematch { get; set; }
        }
        public DbSet<SOPSectionCreator> SOPSectionCreate { get; set; }

        public class SingleLinkTextSec {
            [Key]
            public int SubSecId { get; set; }

            public string UserId { get; set; }
            [ForeignKey("UserId")]
            public virtual ApplicationUser ApplicationUser { get; set; }
            public string valuematch { get; set; }

            public string SingleLinkTextBlock { get; set; }
        }
        public DbSet<SingleLinkTextSec> SingleLinkText { get; set; }

        public class EntSingleLinkTextSec
        {
            [Key]
            public int SubSecId { get; set; }

            public string valuematch { get; set; }

            public string SingleLinkTextBlock { get; set; }
            public string NewTempId { get; set; }
        }
        public DbSet<EntSingleLinkTextSec> UsedSingleLinkText { get; set; }
            
        public class MultilinkTextSec
        {
            [Key]
            public int SubSecId { get; set; }

            public string valuematch { get; set; }
            public string MultilineTextBlock { get; set; }
        }
        public DbSet<MultilinkTextSec> MultilineText { get; set; }

        public class EntMultilinkTextSec
        {
            [Key]
            public int SubSecId { get; set; }

            public string valuematch { get; set; }
            public string MultilineTextBlock { get; set; }
            public string NewTempId { get; set; }
        }
        public DbSet<EntMultilinkTextSec> UsedMultilineText { get; set; }


        public class FileUploadSec
        {
            [Key]
            public int SubSecId { get; set; }

            public string valuematch { get; set; }
            public string FileUrl { get; set; }
        }
        public DbSet<FileUploadSec> FileUpload { get; set; }

        public class TableSec
        {
            [Key]
            public int SubSecId { get; set; }

            public string valuematch { get; set; }
            public string TableHTML { get; set; }
        }
        public DbSet<TableSec> Table { get; set; }

        public class EntTableSec
        {
            [Key]
            public int SubSecId { get; set; }

            public string valuematch { get; set; }
            public string tableval { get; set; }
            public string TableHTML { get; set; }

            public string NewTempId { get; set; }
        }
        public DbSet<EntTableSec> UsedTable { get; set; }

        public class EntTableSecCols
        {
            [Key]
            public int TableCls { get; set; }

            public string tableval { get; set; }
            public string ColText { get; set; }

            public string NewTempId { get; set; }
        }
        public DbSet<EntTableSecCols> UsedTableCols { get; set; }

        public class EntTableSecRows
        {
            [Key]
            public int TableRows { get; set; }
            public string valuematch { get; set; }
            public string tableval { get; set; }
            public string RowText { get; set; }

            public string NewTempId { get; set; }
        }
        public DbSet<EntTableSecRows> UsedTableRows { get; set; }

        public class SecOrder {
            [Key]
            public int SecOrderId { get; set; }

            public int SubSecId { get; set; }

        }
        public DbSet<SecOrder> SectionOrder { get; set; }

        public class DepartmentTable {
            [Key]
            public string DepartmentId { get; set; }

            public string DepartmentName { get; set;  }

            public int CompanyId { get; set; }
            [ForeignKey("CompanyId")]
            public virtual CompanyInfo Company { get; set; }

        }
        public DbSet<DepartmentTable> Departments { get; set; }

        public class JobTitlesTable {
            [Key]
            public string JobTitleId { get; set; }    

            public string JobTitle { get; set; }

            public string TheDepartmentName { get; set; }

            public string DepartmentId { get; set; }
            [ForeignKey("DepartmentId")]
            public virtual DepartmentTable Department { get; set; }

            public int CompanyId { get; set; }
            [ForeignKey("CompanyId")]
            public virtual CompanyInfo Company { get; set; }

        }
        public DbSet<JobTitlesTable> JobTitles { get; set; }

        public class SOPTemplate {
            [Key]
            public string SOPTemplateID { get; set; } 

            public int TopTempId { get; set; }
            [ForeignKey("TopTempId")]
            public virtual SOPTopTemplate SOPTopTemplate { get; set; }

            public string TempName { get; set; }
            public string SOPCode { get; set; }

            public string ExpireDate { get; set; }
        }
        public DbSet<SOPTemplate> SOPNewTemplate { get; set; }

        public class SOPTemplateProcess
        {
            [Key]
            public string SOPTemplateProcessID { get; set; } 

            public int ProcessCount { get; set; }

            public string ProcessType { get; set; }

            public string SOPTemplateID { get; set; }
            [ForeignKey("SOPTemplateID")]
            public virtual SOPTemplate SOPNewTemplate { get; set; }

            public string ProcessName { get; set; }

            public string ProcessDesc { get; set; }

            public string valuematch { get; set;  }

        }
        public DbSet<SOPTemplateProcess> SOPProcess { get; set; }

        public class SOPNewInstance {
            [Key]
            public string InstanceId { get; set; } 

            public string SOPTemplateID { get; set; } 

            public string InstanceRef {get; set;}

            public string InstanceExpire { get; set; }

            public string ProjectId { get; set; }

        }
        public DbSet<SOPNewInstance> NewInstance { get; set; }


        public class InstanceFiles
        {
            [Key]
            public string InstanceFilesID { get; set; } 
            public string InstanceId { get; set; } 

            public string FileString { get; set; } 
        }
        public DbSet<InstanceFiles> InstanceFile { get; set; }

        public class Project {
            [Key]
            public string ProjectId { get; set; }

            public int CompId { get; set; }
            public string ProjectName { get; set; }

            public DateTime creationdate { get; set; }
        }
        public DbSet<Project> Projects { get; set; }

        public class SOPTemplateProcesses
        {
            [Key]
            public int SOPTemplateProcessID { get; set; }

            public int ProcessCount { get; set; }

            public string ProcessType { get; set; }

            public string SOPTemplateID { get; set; }
            [ForeignKey("SOPTemplateID")]
            public virtual SOPTemplate SOPNewTemplate { get; set; }

            public string ProcessName { get; set; }

            public string ProcessDesc { get; set; }

            public string valuematch { get; set; }
        }
        public DbSet<SOPTemplateProcesses> SOPProcessTempls { get; set; }


        public class LiveSOP
        {
            [Key]
            public string LiveSOPId { get; set; }

            public string SOPTemplateID { get; set; }
            public string Id { get; set; }
        }
        public DbSet<LiveSOP> LiveSOPs { get; set; }

        public class SOPInstanceProcess
        {
            [Key]
            public int SOPInstancesProcessId { get; set; }

            public string SOPTemplateID { get; set; }
            public string valuematch { get; set; }
            public string DueDate { get; set; }
            public string ExternalDocument { get; set; }
        }
        public DbSet<SOPInstanceProcess> SOPInstanceProcesses { get; set; }

        public class SOPInstanceProcessFiles
        {
            [Key]
            public int SOPInstanceProcessFilesId { get; set; }

            public int SOPInstancesProcessId { get; set; }
            public string filepath { get; set; }
        }
        public DbSet<SOPInstanceProcessFiles> SOPInstanceProcessesFiles { get; set; }

        public class SOPProcessFiles
        {
            [Key]
            public int FileID { get; set; }

            public string SOPTemplateProcessID { get; set; }

            [DataType(DataType.ImageUrl)]
            public string FilesString { get; set; }
        }
        public DbSet<SOPProcessFiles> ProcessFiles { get; set; }

        public class RACIAccount
        {
            [Key]
            public int RACIAccID { get; set; }

            public int SOPTemplateProcessID { get; set; }

            public string DepartmentId { get; set; }
            [ForeignKey("DepartmentId")]
            public virtual DepartmentTable Department { get; set; }

            public string JobTitleId { get; set; }
            [ForeignKey("JobTitleId")]
            public virtual JobTitlesTable JobTitle { get; set; }

            public string valuematch { get; set; }
            public string soptoptempid { get; set; }
        }
        public DbSet<RACIAccount> SOPRACIAcc { get; set; }

        public class RACIResposible
        {
            [Key]
            public int RACIResID { get; set; }

            public int SOPTemplateProcessID { get; set; }

            public string DepartmentId { get; set; }
            [ForeignKey("DepartmentId")]
            public virtual DepartmentTable Department { get; set; }

            public string JobTitleId { get; set; }
            [ForeignKey("JobTitleId")]
            public virtual JobTitlesTable JobTitle { get; set; }

            public string valuematch { get; set; }

            public string soptoptempid { get; set; }

        }
        public DbSet<RACIResposible> SOPRACIRes { get; set; }

        public class RACIResChosenUser {
            [Key]
            public int RACIResChosenID { get; set; }
            public string RACIResID { get; set; }
            public string Status {get; set;}
            public string StatusComplete { get; set; }
            public int UserId { get; set; }
            public string soptoptempid { get; set; }
        }
        public DbSet<RACIResChosenUser> RACIResUser { get; set; }

        public class RACIResComp
        {
            [Key]
            public int RACIResCompID { get; set; }
            public int RACIResChosenID { get; set; }
            public string StatusComplete { get; set; }
            public string InstanceID { get; set; }
        }
        public DbSet<RACIResComp> RACIResComplete { get; set; }

        public class RACIResRecu
        {
            [Key]
            public int RACIResRecusalID { get; set; }
            public int RACIResChosenID { get; set; }
            public string StatusRecusal { get; set; }
            public string InstanceID { get; set; }
        }
        public DbSet<RACIResRecu> RACIResRecusal { get; set; }

        public class RACIAccChosenUser
        {
            [Key]
            public int RACIAccChosenID { get; set; }
            public string RACIAccID { get; set; }
            public string Status { get; set; }
            public string StatusComplete { get; set; }
            public int UserId { get; set; }
            public string soptoptempid { get; set; }

        }
        public DbSet<RACIAccChosenUser> RACIAccUser { get; set; }

        public class RACIAccComp
        {
            [Key]
            public int RACIAccCompID { get; set; }
            public int RACIAccChosenID { get; set; }
            public string StatusComplete { get; set; }
            public string InstanceID { get; set; }
        }
        public DbSet<RACIAccComp> RACIAccComplete { get; set; }

        public class RACIAccRecu
        {
            [Key]
            public int RACIAccRecusalID { get; set; }
            public int RACIAccChosenID { get; set; }
            public string StatusRecusal { get; set; }
            public string InstanceID { get; set; }
        }
        public DbSet<RACIAccRecu> RACIAccRecusal { get; set; }


        public class RACIConChosenUser
        {
            [Key]
            public int RACIConChosenID { get; set; }
            public string RACIConID { get; set; }
            public string Status { get; set; }
            public string StatusComplete { get; set; }
            public int UserId { get; set; }
            public string soptoptempid { get; set; }
        }
        public DbSet<RACIConChosenUser> RACIConUser { get; set; }

        public class RACIConComp
        {
            [Key]
            public int RACIConCompID { get; set; }
            public int RACIConChosenID { get; set; }
            public string StatusComplete { get; set; }
            public string InstanceID { get; set; }
        }
        public DbSet<RACIConComp> RACIConComplete { get; set; }

        public class RACIConRecu
        {
            [Key]
            public int RACIConRecusalID { get; set; }
            public int RACIConChosenID { get; set; }
            public string StatusRecusal { get; set; }
            public string InstanceID { get; set; }
        }
        public DbSet<RACIConRecu> RACIConRecusal { get; set; }

        public class RACIInfChosenUser
        {
            [Key]
            public int RACIInfChosenID { get; set; }
            public string RACIInfID { get; set; }
            public string Status { get; set; }
            public string StatusComplete { get; set; }
            public int UserId { get; set; }
            public string soptoptempid { get; set; }
        }
        public DbSet<RACIInfChosenUser> RACIInfUser { get; set; }

        public class RACIInfComp
        {
            [Key]
            public int RACIInfCompID { get; set; }
            public int RACIInfChosenID { get; set; }
            public string StatusComplete { get; set; }
            public string InstanceID { get; set; }
        }
        public DbSet<RACIInfComp> RACIInfComplete { get; set; }

        public class RACIInfRecu
        {
            [Key]
            public int RACIInfRecusalID { get; set; }
            public int RACIInfChosenID { get; set; }
            public string InstanceID { get; set; }
            public string StatusRecusal { get; set; }
        }
        public DbSet<RACIInfRecu> RACIInfRecusal { get; set; }

        public class RACIConsulted
        {
            [Key]
            public int RACIConID { get; set; }

            public int SOPTemplateProcessID { get; set; }

            public string DepartmentId { get; set; }
            [ForeignKey("DepartmentId")]
            public virtual DepartmentTable Department { get; set; }

            public string JobTitleId { get; set; }
            [ForeignKey("JobTitleId")]
            public virtual JobTitlesTable JobTitle { get; set; }

            public string valuematch { get; set; }
            public string soptoptempid { get; set; }
        }
        public DbSet<RACIConsulted> SOPRACICon { get; set; }

        public class RACIInformed
        {
            [Key]
            public int RACIInfID { get; set; }

            public int SOPTemplateProcessID { get; set; }
            public string DepartmentId { get; set; }
            [ForeignKey("DepartmentId")]
            public virtual DepartmentTable Department { get; set; }

            public string JobTitleId { get; set; }
            [ForeignKey("JobTitleId")]
            public virtual JobTitlesTable JobTitle { get; set; }

            public string valuematch { get; set; }
            public string soptoptempid { get; set; }
        }
        public DbSet<RACIInformed> SOPRACIInf { get; set; }

        public class ExecuteSop
        {
            [Key]
            public string ExecuteSopID { get; set; }

            public string SectionId { get; set; }
            public string UserId { get; set; }
        }
        public DbSet<ExecuteSop> ExecutedSop { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
               
        }
    }
}
