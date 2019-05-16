using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext ctx)
        {
            _context = ctx;
        }
        public IQueryable<Product> Products => _context.Products;

        public IQueryable<string> Colors => _context.Products.Select(x => x.Color).Distinct().OrderBy(x => x);           
    }
}
