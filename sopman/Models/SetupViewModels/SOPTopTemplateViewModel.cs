using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace sopman.Models.SetupViewModels
{
    public class SOPTopTemplateViewModel
    {
        public int TopTempId { get; set; }
        public string SOPName { get; set; }

        [Display(Name = "Limit Characters")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int SOPNameLimit { get; set; }

        public string SOPCodePrefix { get; set; }

        public string SOPCodeSuffix { get; set; }

        public int SOPAllowCode { get; set; }

        public string SOPAllowCodeLimit { get; set; }

        public virtual IEnumerable<Section> Sections { get; set; }



    }
}
