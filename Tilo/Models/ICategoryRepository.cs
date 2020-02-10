using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> Categories { get; }
        IEnumerable<Category> ParentCategories { get; }
        Task<Category> AddCategoryAsync(string categoryName, string parentCategory);
        Task<Category> DeleteCategoryAsync(int categoryID);
        Task<Category> SaveCategoryAsync(Category category);
    }
}
