using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public interface IFileModelRepository
    {
        IQueryable<FileModel> fileModels { get; }
    }
}
