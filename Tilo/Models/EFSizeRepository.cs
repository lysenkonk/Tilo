using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public class EFSizeRepository : ISizeRepository
    {
        private readonly ApplicationDbContext _context;
        public EFSizeRepository(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public IQueryable<Size> Sizes => _context.Sizes;

        public async Task<Size> SaveSizeAsync(Size size)
        {
            if (size.Id == 0)
            {
                _context.Sizes.Add(size);
            }
            else
            {
                Size dbEntry = _context.Sizes.FirstOrDefault(p => p.Id == size.Id);
                if (dbEntry != null)
                {
                    dbEntry.Name = size.Name;
                    //dbEntry.Path = photo.Path;
                }
            }
            await _context.SaveChangesAsync();
            return size;
        }

        public async Task<Size> RemoveSizeAsync(string sizeName)
        {
            Size dbEntry = _context.Sizes.FirstOrDefault(p => p.Name == sizeName);

            if (dbEntry != null)
            {
                _context.Sizes.Remove(dbEntry);
                await _context.SaveChangesAsync();
            }
            return dbEntry;
        }
    }
}
