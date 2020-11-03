using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models.ViewModels
{
    public class AdminProductViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public List<Size> SizesForCreateProduct { get; set; }
        public IEnumerable<string> Colors { get; set; }
        public IEnumerable<string> SubProductsNames { get; set; }
        public Product Product { get; set; }
    }
}

 