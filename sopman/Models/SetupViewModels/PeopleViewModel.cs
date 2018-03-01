using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using sopman.Models.AccountViewModels;
            
namespace sopman.Models.SetupViewModels
                                             
{
    public class SetupPeopleViewModel
    {
        public List<RegisterSOPUserViewModel> RegisterSOPUser { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
    }
}
