using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
        IQueryable<string> Colors { get; }
    }
}
