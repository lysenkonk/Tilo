using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tilo.Models;
using Tilo.Models.ViewModels;

namespace Tilo.Controllers
{
    public class ShopController : Controller
    {
        private IProductRepository _repository;
        public int PageSize = 6;

        public ShopController(IProductRepository repo)
        {
            _repository = repo;
        }

        public ViewResult Index()
        {
            return View();
        }

        public ViewResult List(string category, int page = 1)
        {
            var viewModel = new ProductsListViewModel
            {
                Products = _repository.Products
                .Where(p => category == null || p.Category.Name == category)
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
                                _repository.Products.Count() :
                                _repository.Products.Where(e => e.Category.Name == category).Count()
                },
                CurrentCategory = category
            };

            return View(viewModel);
        }
    }
}