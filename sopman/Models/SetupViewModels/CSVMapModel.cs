using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace sopman.Models.SetupViewModels
{
    public class CSVMapModel
    {
        public string Email
        {
            get;
            set;
        }
        public string FirstName
        {
            get;
            set;
        }
        public string SecondName
        {
            get;
            set;
        }
        public string Department
        {
            get;
            set;
        }
        public string JobTitle
        {
            get;
            set;
        }
        public CSVMapModel()
        {
            
        }
    }
}
