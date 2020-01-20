using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public interface IPhotoModelRepository
    {
        IQueryable<FileModel> PhotoModels { get; }

        Task<FileModel> SavePhotoModelAsync(FileModel photo);

        Task<FileModel> RemovePhotoModelAsync(string photoName);
    }
}
