using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tilo.Models
{
    public class Product
    {
        public long Id { get; set; } 

        [Required(ErrorMessage = "Please enter a product name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "Please enter a size")]
        public List<Size> Sizes { get; set; }

        [Required]
        [Range(0, int.MaxValue,
            ErrorMessage = "Please enter a positive price")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Please enter a color")]
        public string Color { get; set; }

        //public int CategoryId { get; set; }

        //[Required(ErrorMessage = "Please specify a category")]
        public Category Category { get; set; }

        public List<Product> Products { get; set; }

        public List<FileModel> Images { get; set; } = new List<FileModel>();

        public Product() { }

        public Product(Category category, string name = null)
        {
            Category = category;
            Name = name;
        }
        public Product( string name = null)
        {
            Name = name;
        }
    }
}
