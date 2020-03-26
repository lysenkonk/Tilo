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
        private ICategoryRepository _repoCategories;
        public int PageSize = 10;

        public ShopController(IProductRepository repo, ICategoryRepository categories)
        {
            _repository = repo;
            _repoCategories = categories;
        }
        [Route("Shop")]
        public ViewResult Index()
        {
            //var temp = _repoCategories.Categories.First(p => p.Name == "nh");


            //foreach(var p in _repoCategories.ParentCategories)
            //{
            //    p.ChildCategories = _repoCategories.Categories.Where(e => e.ParentCategory.Name == p.Name).ToList();
            //}
            int count = _repository.Products.Count();
            IQueryable<Product> Products = _repository.Products.Skip(count - 10).Take(10);

            return View(Products);
        }
        [Route("Product/{id}")]
        public ViewResult Product(int id)
        {
            //int categoryId = _repoCategories.Categories.First(p => p.Name == "Трусы");
            Product product = _repository.Products
                .FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return View("Product not found");   
            }
            IQueryable<Product> ProductsWithTheSameNames;                       
            IEnumerable<Product> ProductsWithTheSame = _repository.Products.Where(p => p.Name == product.Name && p.Category != product.Category);
            Dictionary<string, IList<string>> Sizes;
            if (product.Products != null && product.Products.Count > 0)
            {
                Sizes = new Dictionary<string, IList<string>>();
                foreach (var prod in product.Products)
                {
                    ProductsWithTheSameNames = _repository.Products.Where(p => p.Name == prod.Name && p.Category == prod.Category);
                    IList<string> sizes = ProductsWithTheSameNames.Select(x => x.Size).Distinct().OrderBy(x => x).ToList();
                    Sizes.Add(prod.Name, sizes);
                }
            }
            else
            {
                ProductsWithTheSameNames = _repository.Products.Where(p => p.Name == product.Name && p.Category == product.Category);
                IList<string> sizes = ProductsWithTheSameNames.Select(x => x.Size).Distinct().OrderBy(x => x).ToList();
                Sizes = new Dictionary<string, IList<string>>
                {
                    { product.Name,  sizes }
                };
            }
            //foreach(var p in ProductsWithTheSameNames)
            //{

            //}
            var viewModel = new ProductView
            {
                product = product,
                Products = ProductsWithTheSame,
                Sizes = Sizes
            };
            return View(viewModel);
        }
        [Route("Shop/List")]
        [Route("{category}/{page}")]
        public ViewResult List(string category, int page = 1)
        {
            var viewModel = new ProductsViewModel
            {
                Products = _repository.Products
                .Where(p => category == null || p.Category.Name == category)
                .OrderBy(p => p.Id)
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