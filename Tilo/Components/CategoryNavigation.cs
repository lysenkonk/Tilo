using System;  
using System.Collections.Generic;
using System.Linq;
using Tilo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tilo.Components
{
    public class CategoryNavigationViewComponent : ViewComponent
    {
        private ICategoryRepository categoriesRep;

        public CategoryNavigationViewComponent(ICategoryRepository repo)
        {
            categoriesRep = repo;
        }
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(categoriesRep.Categories);
        }
    }
}
