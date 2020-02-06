using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models.ViewModels
{
    public class ProductView
    {
        public Product product { get; set; }

        public IQueryable<string> Sizes { get; set; }
    }
}
