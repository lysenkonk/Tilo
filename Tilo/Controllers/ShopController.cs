using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
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
            IQueryable<Product> Products = _repository.Products.Where(p => p.Category != null).Take(10);
            //IQueryable<Product> Products = _repository.Products.Take(10);

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
            IEnumerable<int> listPricesForSertificate = new List<int>() { 500, 1000, 1500, 2000 }.AsEnumerable();
            ViewBag.PricesForSertivicate = new SelectList(listPricesForSertificate, "Price");
            var viewModel = new ProductView
            {
                product = product,
                Products = ProductsWithTheSame
            };
            return View(viewModel);
        }
        [HttpGet]
        [Route("Shop/List/")]
        [Route("{category}/{page}")]
        public ViewResult List(string category, int page = 1)
        {
            var viewModel = new ProductsViewModel
            {
                Products = _repository.Products
                .Where(p => category == null || p.Category.Name == category)
                .Where(p => p.Category.Name != null)
                .OrderBy(p => p.Id)
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
                CurrentCategory = category
            };
            return View(viewModel);
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