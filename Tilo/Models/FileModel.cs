using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public class FileModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }

        public FileModel(string name)
        {
            Name = name;
        }
        public FileModel()
        {

        }
    }
}
