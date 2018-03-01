using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace sopman.Models.AccountViewModels
{
    public class RegisterSOPUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Second Name")]
        public string SecondName { get; set; }

        public string UserId { get; set; }

        public List<RegisterDepartmentViewModel> Departments { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentId { get; set; }

        public List<RegisterJobTitleViewModel> JobTitles { get; set; }
        public string JobTitle { get; set; }
        public string JobTitleId { get; set; }
        public int ClaimId { get; set; }
        public int CompanyId { get; set; }
        public string Id { get; set; }
    }
}