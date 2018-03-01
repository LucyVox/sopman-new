using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sopman.Models.AccountViewModels
{
    public class AddCompanyInfo
    {
        [Required]
        [Display(Name = "Company Name")]
        public string CompName { get; set; }

        [Display(Name = "Logo")]
        public string Logo { get; set; }

        [Display(Name = "SOP Number Format")]
        public string SOPNumberFormat { get; set; }

        [Display(Name = "SOP Start Number")]
        public string SOPStartNumber { get; set; }

        [Display(Name = "Setting Title")]
        public string SettingTitle { get; set; }

    }
}
