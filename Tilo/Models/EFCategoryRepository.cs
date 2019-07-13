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
            setChildCategories();
        }
        public IQueryable<Category> Categories => context.Categories;
        public IQueryable<Category> ParentCategories => context.Categories.Where(p => p.ParentCategory == null);


        void setChildCategories()
        {
            foreach (var p in ParentCategories)
            {
                p.ChildCategories = context.Categories.Where(e => e.ParentCategory.Name == p.Name).ToList();
            }
        }
    }
}
