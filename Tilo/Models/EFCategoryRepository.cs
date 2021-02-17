using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Tilo.Models;
using Tilo.Services;
using Microsoft.EntityFrameworkCore;

namespace Tilo.Models
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private const string BigFilesFolder = "/Files/Bg/";
        private const string SmallFilesFolder = "/Files/Sm2/";

        private ApplicationDbContext context;
        //private readonly ProductsService _productsService;
        public readonly IProductRepository _repository;
        private readonly IHostingEnvironment _appEnvironment;

        public EFCategoryRepository(ApplicationDbContext ctx/*, ProductsService service*/, IProductRepository repository, IHostingEnvironment appEnvironment)
        {
            context = ctx;
            _repository = repository;
            //_productsService = service;
            SetChildCategories();
            _appEnvironment = appEnvironment;

        }
        public IQueryable<Category> Categories => context.Categories.Include(p => p.ParentCategory);
        public IEnumerable<Category> ParentCategories => context.Categories.Where(p => p.ParentCategory == null);

        void SetChildCategories()
        {
            foreach (var p in ParentCategories)
            {
                p.ChildCategories = context.Categories.Where(e => e.ParentCategory.ID == p.ID).Include(c => c.ChildCategories).ToList();
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
                .FirstOrDefault(p => p.ID == categoryID);

            if (dbEntry == null)
                throw new Exception("404 Not Found Category"); // TODO make proper hadling

            List<Category> categories = new List<Category>(context.Categories.Where(p => p.ParentCategory.Name == dbEntry.Name).ToList());
            List<Product> products = new List<Product>(_repository.Products.Where(p => p.Category.Name == dbEntry.Name).ToList());
            foreach (var product in products)
            {
                    List<FileModel> images = new List<FileModel>();
                    foreach (var image in images)
                    {
                        RemoveImageFiles(image.Name);
                        await _repository.RemoveImageAsync(product.Id, image.Name);
                    }

                    await _repository.DeleteProductAsync(product.Id);
                    //context.Remove(product);
                    //await context.SaveChangesAsync();
                }

            foreach (var category in categories) 
            {
                await DeleteCategoryAsync(category.ID);
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
            if (category.ID == 0)
            {
                context.Categories.Add(category);
            }
            else
            {
                Category dbEntry = context.Categories.FirstOrDefault(c => c.ID == category.ID);

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
        public async Task RemoveImage(int productId, string imageName)
        {
            await _repository.RemoveImageAsync(productId, imageName);
            RemoveImageFiles(imageName);
        }

        private void RemoveImageFiles(string imageName)
        {
            if (File.Exists(_appEnvironment.WebRootPath + BigFilesFolder + imageName))
                File.Delete(_appEnvironment.WebRootPath + BigFilesFolder + imageName);
            else
                return; // TODO make proper hadling

            if (File.Exists(_appEnvironment.WebRootPath + SmallFilesFolder + imageName))
                File.Delete(_appEnvironment.WebRootPath + SmallFilesFolder + imageName);
            else
                return; // TODO make proper hadling
        }
    }
}
