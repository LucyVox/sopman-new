using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sopman.Models.AccountViewModels
{
    public class RegisterDepartmentViewModel
    {

        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        public string DepartmentId { get; set; }


        public string Text { get; set; }
        public string Value { get; set; }
        public string Selected { get; set; }
    }
}
