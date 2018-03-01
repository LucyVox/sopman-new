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
    public class  ProjectList
    {
        public string ProjectName { get; set; }
    }
}
