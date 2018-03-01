using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using sopman.Models.SetupViewModels;

namespace sopman.Models.AccountViewModels {
    public class UserLoggedInViewModels
    {

        [Required]
        [Display(Name = "Company Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Logo Upload")]
        [FileExtensions(Extensions = "jpg,jpeg")]
        public IFormFile Logo { get; set; }

        public List<CompanySetupViewModel> GetCompany { get; set; }

    }


}