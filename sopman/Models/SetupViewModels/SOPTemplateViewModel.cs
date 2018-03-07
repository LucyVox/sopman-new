﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HtmlHelpers.BeginCollectionItemCore;
namespace sopman.Models.SetupViewModels
{
    public class SopTemplate
    {
        [Required]
        [Display(Name = "SOP Name")]
        public string TempName { get; set; }

        [Display(Name = "SOP Code")]
        public string SOPCode { get; set; }

        public int TopTempId { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public string ExpireDate { get; set; }
        public string SectionText { get; set; }
        public string SingleLinkTextBlock { get; set; }
        public string MultilineTextBlock { get; set; }
        public string valuematch { get; set; }
        public string TableHTML { get; set; }

        public List<LineChild> ChildLine { get; set; }
    }
    public class LineChild
    {
        public string SectionText { get; set; }
        public string SingleLinkTextBlock { get; set; }
        public string valuematch { get; set; }
        public string MultilineTextBlock { get; set; }
        public string TableHTML { get; set; }
        [Display(Name = "SOP Name")]
        public string TempName { get; set; }

        [Display(Name = "SOP Code")]
        public string SOPCode { get; set; }

        public int TopTempId { get; set; }
        public string NewTempId { get; set; }
    }
    public class AddTable {
        public int TableRows { get; set; }
        public string tableval { get; set; }
        public string RowText { get; set; }
        public string valuematch { get; set; }
    }

    public class AddTableRow
    {
        public int TableRows { get; set; }
        public string tableval { get; set; }
        public string RowText { get; set; }
        public string valuematch { get; set; }
    }

    public class ShowTemplateViewModel {
        public List<SopTemplate> Template { get; set; }
        public List<LineChild> SingelLine { get; set; }
    }

    public class GetSopTemplate
    {
        public string SectionText { get; set; }
    }

    public class TemplateViewModel
    {
        public IEnumerable<sopman.Models.SetupViewModels.Section> Section { get; set; }
        public sopman.Models.SetupViewModels.SopTemplate SopTemplate { get; set; }
    }

}
