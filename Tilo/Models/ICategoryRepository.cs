using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    interface ICategoryRepository
    {
        IQueryable<Category> Categories { get; }
    }
}
