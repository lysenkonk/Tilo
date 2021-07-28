﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tilo.Models;
using Tilo.Models.ViewModels;

namespace Tilo.Controllers
{
    [ViewComponent(Name = "EditProduct")]
    public class ShopController : Controller
    {
        //private IProductRepository _repository;
        //private AppIdentityDbContext _usersRepo;
        //private ICategoryRepository _repoCategories;
        //private UserManager<IdentityUser> userManager;
        //private readonly int PageSize = 10;
        ////private object userManager;

        //public ShopController(IProductRepository repo, ICategoryRepository categories, AppIdentityDbContext users /*IApplicationBuilder app*/)
        //{
        //    _repository = repo;
        //    _repoCategories = categories;
        //    _usersRepo = users;
        //    //userManager = app.ApplicationServices
        //    // .GetRequiredService<UserManager<IdentityUser>>();
        //}



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
            IQueryable<Product> Products = _repository.Products.Where(p => p.Category != null && p.Category.Name != "Подарочный сертификат").OrderByDescending(p => p.Id).Take(10);


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
            IEnumerable<Product> ProductsWithTheSame = _repository.Products.Where(p => p.Category == product.Category);
            IEnumerable<int> listPricesForSertificate = new List<int>() { 500, 1000, 1500, 2000, 3000, 4000, 5000 }.AsEnumerable();
            ViewBag.PricesForSertivicate = new SelectList(listPricesForSertificate, "Price");
            //string pattern = @"\b\d{1}\.\D+";
            //List<string> descrList = new List<string>();
            //descrList.Add(product.Description.Substring(0, product.Description.IndexOf(':')));
            //string[] str = Regex.Split(product.Description, pattern);
            //Regex reg = new Regex(@"\b\d{1}\.\D+");

            //MatchCollection matches = reg.Matches(product.Description);
            //List<string> str3 = new List<string>();
            //foreach(var st in matches)
            //{
            //    str3.Add(st.ToString());
            //}
            //descrList.AddRange(Regex.Split(product.Description, pattern).ToList());
            //ViewBag.descrList = words.ToList<string>();
            var viewModel = new ProductView
            {
                product = product,
                Products = ProductsWithTheSame
            };
            return View(viewModel);
        }


        [Route("Shop/List/")]
        [Route("Shop/List/{category}")]
        [Route("{category}/{page}")]
        public ViewResult List(string category, int page = 1)
        {            
            IQueryable<Product> products = _repository.Products
               .Where(p => category == null || p.Category.Name == category)
               .Where(p => p.Category.Name != null)
               .OrderBy(p => p.Id);

            if (category == "Купальники")
            {
                products = _repository.Products
               .Where(p => p.Category.ParentCategory.Name == category)
               .Where(p => p.Category.Name != null)
               .OrderBy(p => p.Id);
            }


            if(products.Count() == 0)
            {
                //}
                int count = _repository.Products.Count();
                IQueryable<Product> Products = _repository.Products.Where(p => p.Category != null && p.Category.Name != "Подарочный сертификат").OrderByDescending(p => p.Id).Take(10);
                return View("Index", Products);
            }

            int minPrice = FindMinPrice(_repository.Products
                .Where(p => category == null || p.Category.Name == category)
                .Where(p => p.Category.Name != null).ToList());

            int maxPrice = FindMaxPrice(_repository.Products
                .Where(p => category == null || p.Category.Name == category)
                .Where(p => p.Category.Name != null).ToList());

            if (minPrice > 0 && maxPrice > 0)
            {
                //List<int> listPrices = listPricesForFiltering(minPrice, maxPrice);
                List<int> listPrices = products.Select(p => p.Price).Distinct().ToList();
                //ViewBag.PricesForFiltering = new SelectList(listPrices, "Price");
                ViewBag.PricesForMinFiltering = new SelectList(listPrices, minPrice);
                ViewBag.PricesForMaxFiltering = new SelectList(listPrices, maxPrice);
            }
            List<string> colors = products.Select(p => p.Color).Distinct().ToList();


            var viewModel = new ProductsViewModel
            {
                Products = products
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
                                _repository.Products.Where(p => p.Category.Name != null).Count() :
                                _repository.Products.Where(e => e.Category.Name == category).Count()
                },
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentCategory = category,
                FilterViewModel = new FilterViewModel(colors),
                SortViewModel = new SortViewModel(SortState.PriceAsc)

            };
            return View(viewModel);
        }

    
        [Route("Shop/ListFilteringProducts/")]
        public async Task<IActionResult> ListFilteringProducts(string category, string color, int minPrice = 0, int maxPrice = 0, int page = 1, SortState sortOrder = SortState.PriceAsc)
        {
            //filtering
            IQueryable<Product> products = _repository.Products;
            if (!String.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.Name == category);
            }
            if (category == "Купальники")
            {
                products = _repository.Products
               .Where(p => p.Category.ParentCategory.Name == category)
               .Where(p => p.Category.Name != null)
               .OrderBy(p => p.Id);
            }

            
            if (!String.IsNullOrEmpty(color) && color != "все цвета" )
            {
                products = products.Where(p => p.Color == color);
            }

            if (minPrice == 0)
            {
                minPrice = FindMinPrice(products.ToList());
            }

          
            int max = FindMaxPrice(products.ToList());

            if (maxPrice == 0 || maxPrice < minPrice)
            {
                maxPrice = max;
            }

            if (minPrice > 0 && maxPrice > 0)
            {
                //List<int> listPrices = listPricesForFiltering(0, maxPrice);

                List<int> listPrices =  await products.Select(p => p.Price).Distinct().ToListAsync();
                ViewBag.PricesForMinFiltering = new SelectList(listPrices, minPrice);
                ViewBag.PricesForMaxFiltering = new SelectList(listPrices, maxPrice);


            }

            if (minPrice > 0)
            {
                products = products.Where(p => p.Price >= minPrice);
            }
            if (maxPrice > 0)
            {
                products = products.Where(p => p.Price <= maxPrice);
            }
            List<string> colors = await products.Select(p => p.Color).Distinct().ToListAsync();

            //sorting
            switch (sortOrder)
            {
                case SortState.PriceDesc:
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Price);
                    break;
            }

            var count = await products.CountAsync();
            var items = await products.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
            

            ProductsViewModel viewModel = new ProductsViewModel
            {
                Products = items,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = count
                },
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentCategory = category,
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(colors, color)         

            };
            return View("List", viewModel);
        }





        //[HttpPost]
        //[Route("Shop/ListFilteringProductsByPrice/")]
        //public ViewResult ListFilteringProductsByPrice(ProductsViewModel model, int page = 1)
        //{
        //    var viewModel = new ProductsViewModel
        //    {
        //        Products = _repository.GetFilteringProductsByPrice(model.CurrentCategory, model.MinPrice, model.MaxPrice)
        //        .OrderBy(p => p.Id)
        //        .Skip((page - 1) * PageSize)
        //        .Take(PageSize),
        //        PagingInfo = new PagingInfo
        //        {
        //            CurrentPage = page,
        //            ItemsPerPage = PageSize,
        //            TotalItems = model.CurrentCategory == null ?
        //                        _repository.Products.Where(p => p.Category.Name != null).Count() :
        //                        _repository.Products.Where(e => e.Category.Name == model.CurrentCategory).Count()
        //        },
        //        CurrentCategory = model.CurrentCategory,

        //    };
        //    return View("List", viewModel);
        //}


        private int FindMinPrice(List<Product> repo)
        {
            int minPrice = repo[0].Price;
            foreach (var p in repo)
            {
                if (p.Price < minPrice)
                {
                    minPrice = p.Price;
                }
            }
            return minPrice;
        }

        private int FindMaxPrice(List<Product> repo)
        {
            int maxPrice = repo[0].Price;
            foreach (var p in repo)
            {
                if (p.Price > maxPrice)
                {
                    maxPrice = p.Price;
                }
            }
            return maxPrice;
        }

        private List<int> listPricesForFiltering(int minPrice, int maxPrice)
        {
            List<int> listPrices = new List<int>();
            
            if (minPrice < maxPrice)
            {
                if (minPrice > 0)
                {
                    listPrices.Add(minPrice);
                }
                int currentPrice = minPrice;
                do
                {
                    currentPrice += 100;
                    listPrices.Add(currentPrice);
                } while (currentPrice + 100 < maxPrice);

            }
            listPrices.Add(maxPrice);
            return listPrices;
        }

        public IViewComponentResult Invoke(AdminProductViewModel model)
        {
            return new ViewViewComponentResult();

        }
        //[Route("Shop/Subscribe/")]
        //public async Task<RedirectResult> Subscribe(string email, string returnUrl = "/")
        //{
        //    IdentityUser user;
        //    if (email != null)
        //    {
        //        user =_usersRepo.AspNetUsers.FirstOrDefault(u => u.Email == email);
        //        if(user == null)
        //        {
        //            user = new IdentityUser(email);
        //            user.Email = email;
        //            _usersRepo.AspNetUsers.Add(user);
        //            ViewBag.Text = "Спасибо за подписку!!!";

        //        }
        //    }
        //    await _usersRepo.SaveChangesAsync();
        //    ViewBag.returnUrl = returnUrl;
        //    //int count = _repository.Products.Count();
        //    //IQueryable<Product> Products = _repository.Products.Where(p => p.Category != null).Take(10);
        //    //IQueryable<Product> Products = _repository.Products.Take(10);

        //    return Redirect(returnUrl);
        //}

        //        AppIdentityDbContext identity_context = app.ApplicationServices
        //.GetRequiredService<AppIdentityDbContext>();
        //        identity_context.Database.Migrate();

        //        UserManager<IdentityUser> userManager = app.ApplicationServices
        //         .GetRequiredService<UserManager<IdentityUser>>();

        //        IdentityUser user = await userManager.FindByIdAsync(adminUser);
        //        if (user == null)
        //            user = new IdentityUser("Admin");
        //        await userManager.CreateAsync(user, adminPassword);

    }
    
}