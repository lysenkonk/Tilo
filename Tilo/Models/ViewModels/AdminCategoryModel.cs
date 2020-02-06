using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models.ViewModels
{
    public class AdminCategoryModel
    {
        public IEnumerable<Category> Categories { get; set; }

        public Category Category { get; set; }
    }
}
