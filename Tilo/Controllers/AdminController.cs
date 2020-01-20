using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tilo.Models;
using Tilo.Models.ViewModels;
using Tilo.Services;

namespace Tilo.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductsService _productsService;

        public int pageSize = 20;
        
        public AdminController (ProductsService service)
        {
            _productsService = service;
        }

        //-------------------------------------------------------------------------Catalog actions----------------------------
        public ViewResult Index()
        {
            return View("Index", _productsService.Products);
        }

        public ViewResult List(Category category, int page = 1)
        {
            IEnumerable<Product> products = _productsService.Products
             .Where(p => category == null || p.Category == category);

            var count = products.Count();
            var items = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ProductsViewModel viewModel = new ProductsViewModel
            {
                PagingInfo = new PagingInfo(count, page, pageSize),
                SortViewModel = new SortViewModel(SortState.NameAsc),
                FilterViewModel = new FilterViewModel(_productsService.Categories.ToList(), category),
                Products = items
            };
            return View("~/Views/Admin/Products.cshtml", viewModel.Products);
        }

        public IActionResult Edit(int productId)
        {
            var product = _productsService.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product == null)
                return NotFound();

            var viewModel = new AdminProductViewModel
            {
                Product = product,
                Categories = _productsService.Categories,
                Colors = _productsService.Colors,
                Sizes = _productsService.Sizes
        }; 

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {

            if (ModelState.IsValid)
            {
                await _productsService.SaveProductAsync(product);
                TempData["message"] = $"{product.Name} has been saved";
            }
            var viewModel = new AdminProductViewModel
            {
                Product = product,
                Categories = _productsService.Categories,
                Colors = _productsService.Colors,
                Sizes = _productsService.Sizes
            };
            return View(viewModel);
        }

            public async Task<IActionResult> RemoveImage(int productId, string imageName)
        {
            if (!isProduct(productId))
            {
                return RedirectToAction("Create");
            }

            await _productsService.RemoveImage(productId, imageName);

            return RedirectToAction("Edit", new { productId });
        }

        public async Task<IActionResult> AddImage(int productId, IFormFile uploadedFile)
        {
            if (!isProduct(productId))
            {
                return RedirectToAction("Create");
            }

            await _productsService.AddImage(productId, uploadedFile);

            return RedirectToAction("Edit", new { productId });
        }

        //public IActionResult ListPhotos(int page = 1)
        //{
        //    IEnumerable<PhotoModel> photos = _photosService.Photos.Skip((page - 1) * pageSize).Take(pageSize);

        //    PhotosViewModel viewModel = new PhotosViewModel
        //    {
        //        Photos = _photosService.Photos.Skip((page - 1) * pageSize).Take(pageSize),
        //        PageInfo = new PageInfo(_photosService.Photos.Count(), page, pageSize)
        //    };
        //    return View(viewModel);
        //}

        public IActionResult Create()
        {
            var viewModel = new AdminProductViewModel
            {
                Product = new Product(),
                Categories = _productsService.Categories
            };
            return View("Edit", viewModel);
        }

        public async Task<IActionResult> Delete(int productId)
        {
            Product deletedProduct = await _productsService.DeleteProductAsync(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = $"{deletedProduct.Name} was deleted";
            }
            return RedirectToAction("Index");
        }

        private bool isProduct(int productId)
        {
            var product = _productsService.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
            {
                TempData["message"] = $"That product doesn't exist";
                return false;
            }
            return true;
        }

        public async Task<IActionResult> AddCategory(string categoryName, string parentCategory) 
        {
            var category = _productsService.Categories.FirstOrDefault(c => c.Name == categoryName);

            if (category != null)
            {
                TempData["message"] = $"That category is already exist";
                return RedirectToAction("List");
            }else
            {
                await _productsService.SaveCategoryAsync(categoryName, parentCategory);
                TempData["message"] = $"{categoryName} has been saved";
                return RedirectToAction("List");
            }
        }

    }
}
