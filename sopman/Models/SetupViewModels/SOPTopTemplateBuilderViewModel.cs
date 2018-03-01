using System;
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
    public class SecViewModel {
        public List<Section> Section { get; set; }
        public SingleLineViewModel SingleLine { get; set; }
    }

    public class SOPTopTemplateBuilderViewModel
    {
        /*public SOPTopTemplateBuilderViewModel(){
            Section = new List<Section>();
        } */

        public int TopTempId { get; set; }
        //public List<Section> Sections { get; set; }
        //public List<SingleLine> SingleLine { get; set; }
    }

    public class Section
    {
        public int SectionId { get; set; }
        public string SectionText { get; set; }
        public string TopTempId { get; set; }

        public int SingleLinkTextID { get; set; }
        public string UserId { get; set; }
        public string SingleLinkTextBlock { get; set; }
        public int MultilineTextID { get; set; }

        [DataType(DataType.MultilineText)]
        public string MultilineTextBlock { get; set; }

        public IEnumerable<Section> Sections { get; set; }
        public List<SingleLineViewModel> SingleLines { get; set; }
        public List<MultilineViewModel> MultiLines { get; set; }
        public List<TableViewModel> Table { get; set; }

        public string valuematch { get; set; }

        [Display(Name = "SOP Name")]
        public string TempName { get; set; }

        [Display(Name = "SOP Code")]
        public string SOPCode { get; set; }
    }
    public class SingleLineViewModel {
        public int index { get; set; }
        public int SectionId { get; set; }
        public int SingleLinkTextID { get; set; }
        public string UserId { get; set; }
        public string SingleLinkTextBlock { get; set; }
        public string PreviousFieldId { get; set; }
        public string valuematch { get; set; }
    }
    public class MultilineViewModel
    {
        public int MultilineTextID { get; set; }
        public string MultilineTextBlock { get; set; }
        public string PreviousFieldId { get; set; }
        public string valuematch { get; set; }
    }
    public class TableViewModel
    {
        public int TableSecID { get; set; }
        public string TableHTML { get; set; }
        public string PreviousFieldId { get; set; }
        public string valuematch { get; set; }
    }

}
