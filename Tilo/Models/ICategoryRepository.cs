using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public interface ICategoryRepository
    {
        IQueryable<Category> Categories { get; }
        IQueryable<Category> ParentCategories { get; }
    }
}
