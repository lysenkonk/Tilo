using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public class EFFileModelRepository : IFileModelRepository
    {
        private readonly ApplicationDbContext _context;
        public EFFileModelRepository(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public IQueryable<FileModel> FileModels => _context.FileModels;

        public async Task<FileModel> SavePhotoModelAsync(FileModel photo)
        {
            if (photo.Id == 0)
            {
                _context.FileModels.Add(photo);
            }
            else
            {
                FileModel dbEntry = _context.FileModels.FirstOrDefault(p => p.Id == photo.Id);
                if (dbEntry != null)
                {
                    dbEntry.Name = photo.Name;
                    //dbEntry.Path = photo.Path;
                }
            }
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<FileModel> RemovePhotoModelAsync(string photoName)
        {
            FileModel dbEntry = _context.FileModels.FirstOrDefault(p => p.Name == photoName);

            if (dbEntry != null)
            {
                _context.FileModels.Remove(dbEntry);
                await _context.SaveChangesAsync();
            }
            return dbEntry;
        }
    }
}
