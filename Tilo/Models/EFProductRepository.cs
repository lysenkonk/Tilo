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
        public Product GetProduct(long key) => _context.Products
            .Include(p => p.Images).Include(p => p.Sizes).Include(p => p.Category).Include(p => p.Products).ThenInclude(subProduct => subProduct.Sizes).Include(p => p.Sizes).First(o => o.Id == key);
        //public IEnumerable<Product> Products => _repository.Products.Include(p => p.Images).Include(p => p.Category).Include(p => p.Products).ThenInclude(subProduct => subProduct.Sizes).Include(p => p.Sizes);

        public IQueryable<Product> Products =>  _context.Products.Include(p => p.Images).Include(p => p.Category).Include(p => p.Products).ThenInclude(subProduct => subProduct.Sizes).Include(p => p.Sizes);

        //public IQueryable<string> Colors => _context.Products.Select(x => x.Color).Distinct().OrderBy(x => x);
        public IEnumerable<string> Colors => new string[]{"чёрный", "белый", "красный", "зелёный", "синий", "айвори","марсала", "оранжевый", "розовый", "желтый"};
        //public IEnumerable<string> Sizes => new string[] { "70", "75", "80", "A", "B", "C", "D", "E", "XS", "S", "M", "L" };

        public IEnumerable<Category> Categories => _context.Categories;

        public async Task<Product> SaveProductAsync(Product product)
        {
            Category category = null;
            Product dbEntry = null;
            if (product.Category != null)
            {
                category = _context.Categories.FirstOrDefault(c => c.Name == product.Category.Name);
                if (category != null)
                {
                    product.Category = _context.Categories.FirstOrDefault(c => c.Name == product.Category.Name);
                }
            }
            if (product.Id == 0)
            {              
                _context.Products.Add(product);
            }
            else
            {
                dbEntry = _context.Products.FirstOrDefault(p => p.Id == product.Id);

                if (dbEntry != null)
                {
                    
                    if(category != null)
                    {
                        dbEntry.Category = _context.Categories.FirstOrDefault(c => c.Name == product.Category.Name);
                    }
                    else
                    {
                        if (product.Category != null)
                        {
                            dbEntry.Category = product.Category;
                        }
                    }
               
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Color = product.Color;


                    //if(product.Products != null)
                    //{
                    //    //dbEntry.Products = new List<Product>(product.Products);
                    //    foreach(var p in product.Products)
                    //    {
                              //dbEntry.Category = _context.Categories.FirstOrDefault(c => c.Name == product.Category.Name);
                    //        foreach (var s in p.Sizes)
                    //        {
                    //            Size theSameSize = dbEntry.Sizes.FirstOrDefault(size => size.Name == s.Name);
                    //            if (theSameSize == null)
                    //            {
                    //                dbEntry.Sizes.Add(s);
                    //            }
                    //        }


                    //        //foreach (var current in dbEntry.Products)
                    //        //{
                    //        //    if(p.Id == current.Id)
                    //        //    {
                    //        //        p.Sizes = current.Sizes;
                    //        //    }
                    //        //}
                    //    }
                    //}
                    if (product.Sizes != null)
                    {
                        foreach (var s in product.Sizes)
                        {
                            Size theSameSize = dbEntry.Sizes.FirstOrDefault(size => size.Name == s.Name);
                            if (theSameSize == null)
                            {
                                dbEntry.Sizes.Add(s);
                            }
                        }
                    }
                    if (dbEntry.Sizes != null && product.Sizes == null)
                    {
                        product.Sizes = new List<Size>(dbEntry.Sizes);

                    }
                    if (product.Products != null && dbEntry.Products != null)
                    {
                        foreach (var p in product.Products)
                        {
                            Product theSameProduct = dbEntry.Products.FirstOrDefault(cur => cur.Id == p.Id);
                            if (theSameProduct.Sizes != null)
                            {
                                if (p.Sizes == null)
                                {
                                    p.Sizes = new List<Size>(theSameProduct.Sizes);
                                }
                                else p.Sizes = theSameProduct.Sizes;
                            }
                        }
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> DeleteProductAsync(long productID)
        {
            Product dbEntry = _context.Products
                .FirstOrDefault(p => p.Id == productID);
            if (dbEntry != null)
            {
                _context.Products.Remove(dbEntry);
                await _context.SaveChangesAsync();
            }

            return dbEntry;
        }
         public async Task<Product> RemoveSizeAsync(long productId, List<string> sizes)
        {
            Product dbEntry = _context.Products.Include(p => p.Sizes).FirstOrDefault(p => p.Id == productId);

            if (dbEntry == null)
                throw new Exception("404 Not Found product");
            Size sizeEntry;
            foreach (var size in sizes)
            {
                sizeEntry = dbEntry.Sizes.FirstOrDefault(s => s.Name == size);
                if (sizeEntry == null)
                    throw new Exception("404 Not Found Image");
                dbEntry.Sizes.Remove(sizeEntry);
                await _context.SaveChangesAsync();
            }
            return dbEntry;
        }

        public async Task<FileModel> AddImageAsync(long productId, FileModel image)
        {
            Product dbEntry = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == productId);

            if (dbEntry == null)
                throw new Exception("404 Not Found product");

            dbEntry.Images.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<FileModel> RemoveImageAsync(long productId, string fileName)
        {
            Product dbEntry = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == productId);

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
