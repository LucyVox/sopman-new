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
    public class SOPOverView
    {
        public int SectionId { get; set; }
        public string SectionText { get; set; }
        public string valuematch { get; set; }
        public string SOPTemplateID { get; set; }
        public string ExecuteSopID { get; set; }
        public string Status { get; set; }
        public string StatusComplete { get; set; }
        public string SingleLinkTextBlock { get; set; }
        public string MultilineTextBlock { get; set; }
        public string TableHTML { get; set; }
        public string RowText { get; set; }
        public string tableval { get; set; }
        public string ColText { get; set; }
        public string ProcessName { get; set; }
        public string ProcessType { get; set; }
        public string ProcessDesc { get; set; }
        public string DueDate { get; set; }
        public string ExternalDocument { get; set; }
        public string DeparmentName { get; set; }
        public string JobTitle { get; set; }
        public string soptoptempid { get; set; }
        public int ClaimId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public int RACIResChosenID { get; set; }
        public string RACIResID { get; set; }
        public int RACIAccChosenID { get; set; }
        public string RACIAccID { get; set; }
        public int RACIInfChosenID { get; set; }
        public string RACIInfID { get; set; }
        public int RACIConChosenID { get; set; }
        public string RACIConID { get; set; }

        public int UserId { get; set; }
        public string NewTempId { get; set; }
        public string StatusRecusal { get; set; }
        public string InstanceID { get; set; }
    }
}
