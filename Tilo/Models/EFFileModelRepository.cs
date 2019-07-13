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

        public IQueryable<FileModel> fileModels => _context.FileModels;
    }
}
