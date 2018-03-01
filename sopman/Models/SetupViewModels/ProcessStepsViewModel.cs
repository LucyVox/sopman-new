using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HtmlHelpers.BeginCollectionItemCore;
using sopman.Models.AccountViewModels;
namespace sopman.Models.SetupViewModels
{
    public class ProcessStepsViewModel
    {
        public List<Process> Processes { get; set; } = new List<Process>();
        public List<SubProcess> SubProcesses { get; set; } = new List<SubProcess>();
    }

    public class Process {
        public int SOPTemplateProcessID { get; set; } 

        public string ProcessName { get; set; }
        public string ProcessDesc { get; set; }

        [Display(Name = "Upload files")]
        [FileExtensions(Extensions = "jpg,jpeg,png,doc,docx,pdf,xls,xlsx")]
        public IFormFile FilesString { get; set; }
        public List<RACIResponsible> Responsible { get; set; }
        public List<RACIAccountable> Accountable { get; set; }
        public List<RACIInformed> Informed { get; set; }
        public List<RACIConsulted> Consulted { get; set; }

        public string valuematch { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public string SOPTemplateID { get; set; }
    }
    public class SubProcess
    {
        public int SOPTemplateProcessID { get; set; }

        public string ProcessName { get; set; }
        public string ProcessDesc { get; set; }


        [Display(Name = "Upload files")]
        [FileExtensions(Extensions = "jpg,jpeg,png,doc,docx,pdf,xls,xlsx")]
        public IFormFile FilesString { get; set; }
        public List<RACIResponsible> Responsible { get; set; }
        public List<RACIAccountable> Accountable { get; set; }
        public List<RACIInformed> Informed { get; set; }
        public List<RACIConsulted> Consulted { get; set; }

        public string valuematch { get; set; }
    }

    public class RACIResponsible {
        public int RACIResID { get; set; }
        public string ProcessName { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public string valuematch { get; set; }
        public List<RegisterDepartmentViewModel> Departments { get; set; }
        public List<RegisterJobTitleViewModel> JobTitles { get; set; }
    }
    public class mainvaluematch {
        public string valuematch { get; set; }
    }

    public class RACIAccountable {
        public int RACIAccID { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public List<RegisterDepartmentViewModel> Departments { get; set; }
        public List<RegisterJobTitleViewModel> JobTitles { get; set; }
        public string valuematch { get; set; }
        public int SOPTemplateProcessID { get; set; }
    }

    public class RACIInformed
    {
        public int RACIInfID { get; set; }
        public string ProcessName { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public string valuematch { get; set; }
        public List<RegisterDepartmentViewModel> Departments { get; set; }
        public List<RegisterJobTitleViewModel> JobTitles { get; set; }
        public int SOPTemplateProcessID { get; set; }
    }
    public class RACIConsulted
    {
        public int RACIConId { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public List<RegisterDepartmentViewModel> Departments { get; set; }
        public List<RegisterJobTitleViewModel> JobTitles { get; set; }
        public string valuematch { get; set; }
        public int SOPTemplateProcessID { get; set; }
    }
}
