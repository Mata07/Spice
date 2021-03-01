using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public SubCategory SubCategory { get; set; }

        // We need only List of Names from SubCategory so it's <string> not full SubCategory
        public List<string> SubCategoryList { get; set; }

        // for error messages
        public string StatusMessage { get; set; }

    }
}
