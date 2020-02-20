using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tilo.Models
{
    public class Category : IComparable<Category>
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "Please enter a category name")]
        public string Name { get; set; }
        public Category ParentCategory { get; set; }

        public List<Category> ChildCategories { get; set; }
        //public List<Product> Products { get; set; }

        public Category(string name, Category parentCategory = null)
        {
            Name = name;
            if(parentCategory != null)
            {
                ParentCategory = parentCategory; 
            }
        }
        public Category()
        {

        }

        public int CompareTo(Category otherCategory)
        {
            if (otherCategory == null) return 1;

            return this.Name.CompareTo(otherCategory.Name);
        }
    }
}
