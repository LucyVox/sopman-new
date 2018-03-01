using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using sopman.Data;
using Microsoft.AspNetCore.Http;
using HtmlHelpers.BeginCollectionItemCore;
namespace sopman.Models.SetupViewModels
{
    public class SectionViewModel
    {
        public int SectionId { get; set; }
        public string SectionText { get; set; }

        public string TopTempId { get; set; }

        public List<Section> Sections { get; set; }
    }
}
