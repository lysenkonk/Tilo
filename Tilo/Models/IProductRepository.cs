using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    interface IProductRepository
    {
        IQueryable<Product> Products { get; }
    }
}
