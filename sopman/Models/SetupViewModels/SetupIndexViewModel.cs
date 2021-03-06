﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace sopman.Models.SetupViewModels
{
    public class SetupIndexViewModel
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

        //public string Logo { get; set; }

        [Display(Name = "SOP Number Format")]
        public string SOPNumberFormat { get; set; }

        [Display(Name = "SOP Start Number")]
        public string SOPStartNumber { get; set; }

        [Display(Name = "Setting Title")]
        public string SettingTitle { get; set; }

    }
}
