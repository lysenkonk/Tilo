using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tilo.Models;
using Tilo.Models;

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
        public IEnumerable<Category> Categories => context.Categories;
        public IEnumerable<Category> ParentCategories => context.Categories.Where(p => p.ParentCategory == null);

        void setChildCategories()
        {
            foreach (var p in ParentCategories)
            {
                p.ChildCategories = context.Categories.Where(e => e.ParentCategory.CategoryID == p.CategoryID).ToList();
            }
        }
        public async Task<Category> AddCategoryAsync(string categoryName, string categoryParent)
        {
            Category category;
            Category parent = context.Categories.FirstOrDefault(c => c.Name == categoryParent);
            if (parent != null)
            {
                category = new Category(categoryName, parent);
            }
            else
            {
                category = new Category(categoryName);
            }

            context.Categories.Add(category);

            await context.SaveChangesAsync();
            return category;
        }


        public async Task<Category> DeleteCategoryAsync(int categoryID)
        {
            Category dbEntry = context.Categories
                .FirstOrDefault(p => p.CategoryID == categoryID);

            if (dbEntry == null)
                throw new Exception("404 Not Found Category"); // TODO make proper hadling

            List<Category> categories = new List<Category>(context.Categories.Where(p => p.ParentCategory.Name == dbEntry.Name).ToList());
            List<Product> products = new List<Product>(context.Products.Where(p => p.Category.Name == dbEntry.Name).ToList());
            foreach (var product in products)
            {
                context.Remove(product);
                await context.SaveChangesAsync();
            }

            foreach (var category in categories) 
            {
                await DeleteCategoryAsync(category.CategoryID);
                await context.SaveChangesAsync();
            }

            if (dbEntry != null)
            {
                context.Categories.Remove(dbEntry);
                await context.SaveChangesAsync();
            }
            return dbEntry;
        }

        public async Task<Category> SaveCategoryAsync(Category category)
        {
            if (category.CategoryID == 0)
            {
                context.Categories.Add(category);
            }
            else
            {
                Category dbEntry = context.Categories.FirstOrDefault(c => c.CategoryID == category.CategoryID);

                if (dbEntry != null)
                {
                    if (dbEntry.ParentCategory != null)
                    {
                        Category categoryParent = context.Categories.FirstOrDefault(c => c.Name == category.ParentCategory.Name);
                        if (categoryParent != null)
                        {
                            dbEntry.ParentCategory = categoryParent;
                        }
                        else
                        {
                            dbEntry.ParentCategory = new Category(category.ParentCategory.Name);
                        }
                    }

                    dbEntry.Name = category.Name;
                }
            }
            await context.SaveChangesAsync();
            return category;
        }
    }
}
