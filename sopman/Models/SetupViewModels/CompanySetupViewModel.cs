using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace sopman.Models.SetupViewModels
{
    public class CompanySetupViewModel
    {

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Logo Upload")]
        [FileExtensions(Extensions = "jpg,jpeg")]
        public IFormFile Logo { get; set; }

        [Required]
        [Display(Name = "Logo Upload")]
        [FileExtensions(Extensions = "jpg,jpeg, png")]
        public IFormFile File { get; set; }


        public string FileURL { get; set; }
        //public string Logo { get; set; }

        [Display(Name = "SOP Number Format")]
        public string SOPNumberFormat { get; set; }

        [Display(Name = "SOP Start Number")]
        public string SOPStartNumber { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Second Name")]
        public string SecondName { get; set; }


    }

}
