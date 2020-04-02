using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public class Size
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public Size(string name)
        {
            Name = name;
        }
        public Size()
        {

        }
    }
}
