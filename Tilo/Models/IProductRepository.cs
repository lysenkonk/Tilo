using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }


        IQueryable<Product> GetFilteringProductsBySize(string category, string size);
        IQueryable<Product> GetFilteringProductsByColor(string category, string color);
        IQueryable<Product> GetFilteringProductsByPrice(string category, int? minPrice, int? maxPrice);

        IEnumerable<string> Colors { get; }
        //IEnumerable<string> Sizes { get; }

        IEnumerable<Category> Categories { get; }

        Task<Product> SaveProductAsync(Product product);

        Task<Product> DeleteProductAsync(long productID);

        Task<FileModel> AddImageAsync(long productId, FileModel image);

        Task<FileModel> RemoveImageAsync(long productId, string fileName);

        Task<Product> RemoveSizeAsync(long productId, List<string> sizes);
    }
}
