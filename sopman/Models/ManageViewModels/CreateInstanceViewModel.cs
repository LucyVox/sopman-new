using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HtmlHelpers.BeginCollectionItemCore;
namespace sopman.Models.ManageViewModels
{
    public class CreateInstanceViewModel
    {
        public string SOPTemplateID { get; set; }
        public string TempName { get; set; }
        public string SOPCode { get; set; }

        public string ExpireDate { get; set; }
        public string InstanceRef { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public string InstanceExpire { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }
        public List<ProjectsList> Projects { get; set; }
    }
    public class ProjectsList
    {
        public string ProjectId { get; set; }
        public int CompId { get; set; }
        public string ProjectName { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class InstanceProcessViewModel
    {
        public string SOPTemplateID { get; set; }
        public string TempName { get; set; }
        public string SOPCode { get; set; }
        public string ProjectName { get; set; }
        public string ExpireDate { get; set; }
        public string InstanceRef { get; set; }

        public List<ProcessOutput> Processes { get; set; }
    }

    public class ProcessOutput
    {
        public string SOPTemplateID { get; set; }
        public int RACIResChosenID { get; set; }
        public int RACIAccChosenID { get; set; }
        public int RACIConChosenID { get; set; }
        public int RACIInfChosenID { get; set; }
        public string ProcessName { get; set; }
        public string ProcessDesc { get; set; }
        public string valuematch { get; set; }
        public string ProcessType { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public string InstanceId { get; set; }

        [DataType(DataType.Date)]
        public string DueDate { get; set; }
        public string ExternalDocument { get; set; }

        [Display(Name = "File upload")]
        [FileExtensions(Extensions = "jpg,jpeg,png,pdf,docx,doc")]
        public IFormFile FilesString { get; set; }

        public List<RACIRESPick> RACIRes { get; set; } 

        public int RACIResID { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitle { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public int ClaimId { get; set; }
    }
    public class RACIRESPick
    {
        public List<RACIRESList> RACIRESList { get; set; } 
        public string RACIResID { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public string valuematch { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitle { get; set; }
    }
    public class RACIRESList
    {
        public string RACIResID { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public string valuematch { get; set; }

    }

}
