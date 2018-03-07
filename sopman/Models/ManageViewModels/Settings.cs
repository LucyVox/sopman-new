using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace sopman.Models.ManageViewModels
{
    public class SettingsViewModel
    {

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Display(Name = "Company Name")]
        public string Name { get; set; }


        [Display(Name = "Logo Upload")]
        [FileExtensions(Extensions = "jpg,jpeg,png")]
        public IFormFile File { get; set; }

        //public string Logo { get; set; }

        [Display(Name = "SOP Number Format")]
        public string SOPNumberFormat { get; set; }

        [Display(Name = "SOP Start Number")]
        public string SOPStartNumber { get; set; }


        [Display(Name = "First Name")]
        public string FirstName { get; set; }

       
        [Display(Name = "Second Name")]
        public string SecondName { get; set; }


    }

}



