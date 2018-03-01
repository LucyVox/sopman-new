using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using sopman.Data;

namespace sopman.Models.AccountViewModels
{
    public class RegisterJobTitleViewModel
    {
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        public string JobTitleId { get; set; } 

        public string TheDepartmentName { get; set; } 

        public List<RegisterDepartmentViewModel> Departments { get; set; }

        public string DepartmentName { get; set; }
        public string DepartmentId { get; set; }


        public string Text { get; set; }
        public string Value { get; set; }
    }
}
