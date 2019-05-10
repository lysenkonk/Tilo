using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private ApplicationDbContext context;

        public EFCategoryRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Category> Categories => context.Categories;
    }
}
