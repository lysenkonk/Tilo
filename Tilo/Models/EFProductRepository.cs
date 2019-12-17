using Microsoft.EntityFrameworkCore;
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
        public IQueryable<Product> Products => _context.Products.Include(p => p.Images).Include(p => p.Category);

        public IQueryable<string> Colors => _context.Products.Select(x => x.Color).Distinct().OrderBy(x => x);
        public IQueryable<string> Sizes => _context.Products.Select(x => x.Size).Distinct().OrderBy(x => x);

        public IEnumerable<Category> Categories => _context.Categories;

        public async Task<Product> SaveProductAsync(Product product)
        {
            if (product.ProductID == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                Product dbEntry = _context.Products.FirstOrDefault(p => p.ProductID == product.ProductID);

                if (dbEntry != null)
                {
                    Category category = _context.Categories.FirstOrDefault(c => c.Name == product.Category.Name);
                    if(category != null)
                    {
                        dbEntry.Category = category;
                    }
                    else
                    {
                        dbEntry.Category = new Category(product.Category.Name);
                    }
               
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Color = product.Color;
                    dbEntry.Size = product.Size;
                }
            }
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> DeleteProductAsync(int productID)
        {
            Product dbEntry = _context.Products
                .FirstOrDefault(p => p.ProductID == productID);
            if (dbEntry != null)
            {
                _context.Products.Remove(dbEntry);
                await _context.SaveChangesAsync();
            }

            return dbEntry;
        }

        public async Task<FileModel> AddImageAsync(int productId, FileModel image)
        {
            Product dbEntry = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.ProductID == productId);

            if (dbEntry == null)
                throw new Exception("404 Not Found product");

            dbEntry.Images.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<FileModel> RemoveImageAsync(int productId, string fileName)
        {
            Product dbEntry = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.ProductID == productId);

            if (dbEntry == null)
                throw new Exception("404 Not Found product");

            FileModel image = dbEntry.Images.FirstOrDefault(img => img.Name == fileName);

            if (image == null)
                throw new Exception("404 Not Found Image");

            dbEntry.Images.Remove(image);
            await _context.SaveChangesAsync();
            return image;
        }
    }
}
