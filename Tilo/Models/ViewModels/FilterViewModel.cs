using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models.ViewModels
{
    public class FilterViewModel
    {
        //public FilterViewModel(List<Category> categories, Category category)
        //{
        //    categories.Insert(0, new Category("all"));
        //    Categories = new SelectList(categories, "Name", category.Name);
        //    SelectedCategory = category;
        //}

        //public SelectList Categories { get; private set; }
        //public Category SelectedCategory { get; private set; }

        public FilterViewModel(List<string> colors, string color = "все цвета")
        {
            colors.Insert(0, "все цвета");
            Colors = new SelectList(colors, color);
            SelectedColor = color;
        }

        public SelectList Colors { get; private set; }
        public string SelectedColor { get; private set; }
    }
}
