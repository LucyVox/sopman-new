using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sopman.Models.RoleViewModels
{
    public class RegisterNewRoles
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}



