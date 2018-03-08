using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sopman.Models.AccountViewModels
{
    public class SearchViewModel {
        public string TempName { get; set; }
        public List<SearchResultsViewModel> SearchRes { get; set; }
    }


    public class SearchResultsViewModel {
        public string TempName { get; set; }
    }
}

