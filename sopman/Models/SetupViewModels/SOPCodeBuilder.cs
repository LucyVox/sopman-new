using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace sopman.Models.SetupViewModels
{
    public class SOPCodeBuilder
    {
        public int SOPNumberingId { get; set; }

        [Required]
        public string InputValue { get; set; }
        public int CompanyId { get; set; }
    }
}
