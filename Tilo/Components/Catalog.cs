using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tilo.Models;

namespace Tilo.Components
{
    public class CatalogViewComponent : ViewComponent
    {
        private ICategoryRepository _repoCategories;

        public CatalogViewComponent(ICategoryRepository categories)
        {
            _repoCategories = categories;
        }

        public IViewComponentResult Invoke()
        {
            return View(_repoCategories);
        }
    }
}
