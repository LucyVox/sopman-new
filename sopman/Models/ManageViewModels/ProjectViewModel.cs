using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace sopman.Models.ManageViewModels
{
    public class ProjectViewModel
    {
        public string ProjectId { get; set; }
        public int CompId { get; set; }
        public string ProjectName { get; set; }

    }
}
