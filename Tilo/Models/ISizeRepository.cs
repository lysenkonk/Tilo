using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public interface ISizeRepository
    {
        IQueryable<Size> Sizes { get; }

        Task<Size> SaveSizeAsync(Size size);

        Task<Size> RemoveSizeAsync(string sizeName);
    }
}
