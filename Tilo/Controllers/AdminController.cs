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

        public ViewResult Categories()
        {
            return View("Categories", _productsService.Categories);
        }

        public ViewResult List(string category, int page = 1)
        {
            IEnumerable<Product> products = _productsService.Products
             .Where(p => category == null || p.Category.Name == category);


            //IEnumerable<Product> Products = _productsService.Products
            //    .Where(p => category == null || p.Category.Name == category)
            //    .OrderBy(p => p.ProductID)
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize);

            //var count = products.Count();
            //var items = Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View("Index", products);
        }

        public IActionResult Edit(int productId)
        {
            var product = _productsService.Products.FirstOrDefault(p => p.ID == productId);

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
            Product product = new Product();
            product.Category = _productsService.Categories.FirstOrDefault();

            var viewModel = new AdminProductViewModel
            {
                Product = product,
                Categories = _productsService.Categories,
                Colors = _productsService.Colors,
                Sizes = _productsService.Sizes
            };
            return View("Edit", viewModel);
        }


        public IActionResult CreateCategory()
        {
            var viewModel = new AdminCategoryModel
            {
                Category = new Category("Name", new Category("ParentName")), 
                Categories = _productsService.Categories
            };
            return View("Category", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            category.ParentCategory = category.ParentCategory.Name == "ParentName" ? null : category.ParentCategory;

            var c = _productsService.Categories.SingleOrDefault(curent => curent.Name == category.Name && curent.ParentCategory.Name == category.ParentCategory.Name);
            //string nameParentCategory = category.ParentCategory.Name == "ParentName" ? null : category.ParentCategory.Name;

            if (c != null) 
            {
                if (c.ParentCategory != null && c.ParentCategory.Name != null)
                {
                    if (c.Name == category.Name && category.ParentCategory.Name == c.ParentCategory.Name)
                        TempData["message"] = $"That category is already exist";
                }else
                {
                    TempData["message"] = $"That category is already exist";
                }
            }
            else
            {
                if (category.ParentCategory != null)
                {
                    await _productsService.SaveCategoryAsync(category.Name, category.ParentCategory.Name);
                }
                else
                    await _productsService.SaveCategoryAsync(category.Name);

                TempData["message"] = $"{category.Name} has been saved";
            }
            return RedirectToAction("Index");
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

        public async Task<IActionResult> DeleteCategory(int categoryID)
        {
            Category deletedCategory = await _productsService._categoryRepository.DeleteCategoryAsync(categoryID);
            if (deletedCategory != null)
            {
                TempData["message"] = $"{deletedCategory.Name} was deleted";
            }
            return RedirectToAction("Categories");
        }


        public IActionResult EditCategory(int categoryID)
        {
            var category = _productsService._categoryRepository.Categories.FirstOrDefault(c => c.CategoryID == categoryID);

            if (category == null)
                return NotFound();

            var viewModel = new AdminCategoryModel
            {
                Category = category,
                Categories = _productsService.Categories
            };

            return View("Category", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {

            if (ModelState.IsValid)
            {
                await _productsService._categoryRepository.SaveCategoryAsync(category);
                TempData["message"] = $"{category.Name} has been saved";
            }
            var viewModel = new AdminCategoryModel
            {
                Category = category,
                Categories = _productsService.Categories
            };
            return View("Category", viewModel);
        }





        private bool isProduct(int productId)
        {
            var product = _productsService.Products.FirstOrDefault(p => p.ID == productId);
            if (product == null)
            {
                TempData["message"] = $"That product doesn't exist";
                return false;
            }
            return true;
        }

        //public async Task<IActionResult> AddCategory(string categoryName, string parentCategory = null) 
        //{
        //    var category = _productsService.Categories.FirstOrDefault(c => c.Name == categoryName);

        //    if (category != null)
        //    {
        //        TempData["message"] = $"That category is already exist";
        //        return RedirectToAction("Index");
        //    }else
        //    {
        //        await _productsService.SaveCategoryAsync(categoryName, parentCategory);
        //        TempData["message"] = $"{categoryName} has been saved";
        //        return RedirectToAction("Index");
        //    }
        //}

    }
}
