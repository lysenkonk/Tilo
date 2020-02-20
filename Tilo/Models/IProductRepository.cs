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

        IEnumerable<Category> Categories { get; }

        Task<Product> SaveProductAsync(Product product);

        Task<Product> DeleteProductAsync(long productID);

        Task<FileModel> AddImageAsync(long productId, FileModel image);

        Task<FileModel> RemoveImageAsync(long productId, string fileName);
    }
}
