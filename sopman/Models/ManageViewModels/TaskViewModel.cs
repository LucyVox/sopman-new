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
    public class TasksViewModel
    {
        public string SOPTemplateID { get; set; }

        public int TopTempId { get; set; }
        public string TempName { get; set; }
        public string SOPCode { get; set; }
        public string ExpireDate { get; set; }

        public string InstanceRef { get; set; }
        public string InstanceExpire { get; set; }
        public string ProjectId { get; set; }

        public string valuematch { get; set; }
        public string soptoptempid { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public string InstanceId { get; set; }
        public DateTime editDate { get; set; }
        public int RACIResID { get; set; }
        public int RACIAccID { get; set; }
        public int RACIConID { get; set; }
        public int RACIInfID { get; set; }
        public string ProcessName { get; set; }
        public string Projectname { get; set; }

        public string ExecuteSopID { get; set; }
        public string SectionId { get; set; }
    }
}
