 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tilo.Models;
using Tilo.Models.ViewModels;
using Tilo.Services;

namespace Tilo.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ProductsService _productsService;

        public int pageSize = 20;
        
        public AdminController (ProductsService service)
        {
            _productsService = service;
        }

        //-------------------------------------------------------------------------Catalog actions----------------------------
        [Route("Admin")]
        public ViewResult Index()
        {
            return View("Index", _productsService.Products);
        }
        [Route("Admin/Categories")]
        public ViewResult Categories()
        {
            return View("Categories", _productsService.Categories);
        }
        [Route("Admin/List")]
        public ViewResult List(string category, int page = 1)
        {
            IEnumerable<Product> products = _productsService.Products
             .Where(p => category == null || p.Category?.Name == category).Where(p => p.Category != null);


            //IEnumerable<Product> Products = _productsService.Products
            //    .Where(p => category == null || p.Category.Name == category)
            //    .OrderBy(p => p.ProductID)
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize);

            //var count = products.Count();
            //var items = Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View("Index", products);
        }
        [Route("Admin/Edit")]
        public IActionResult Edit(int productId)
        {
            var product = _productsService.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
                return NotFound();

            var viewModel = new AdminProductViewModel

            {
                Product = product,
                Categories = _productsService.Categories,
                Colors = _productsService.Colors,
                SizesForCreateProduct = _productsService.SizesForCreateProduct
        }; 

            return View(viewModel);
        }

        [HttpPost]
        [Route("Admin/Edit")]
        public async Task<IActionResult> Edit(Product product, List<string> sizes, List<string> Top, List<string> Bottom)
        {
            Product productCurrent = _productsService.Products.FirstOrDefault(p => p.Id == product.Id);
            //if(productCurrent.Sizes)


            if (ModelState.IsValid)
            {
                if(product.Sizes == null && sizes.Count > 0)
                {
                    product.Sizes = new List<Size>();
                }
               foreach(var s in sizes)
                {
                    Size theSameSize = productCurrent.Sizes.FirstOrDefault(size => size.Name == s);
                    if (theSameSize == null)
                    {
                        product.Sizes.Add(new Size(s));
                    }
                }

                if (Top.Count > 0)
                {
                    Product prod = product.Products.FirstOrDefault(p => p.Name == "Top");
                    if(prod.Sizes == null)
                    {
                        prod.Sizes = new List<Size>();
                    }
                    foreach (var s in Top)
                    {
                        prod.Sizes.Add(new Size(s));
                    }
                }
                if (Bottom.Count > 0)
                {
                    Product prod = product.Products.FirstOrDefault(p => p.Name == "Bottom");
                    if (prod.Sizes == null)
                    {
                        prod.Sizes = new List<Size>();
                    }
                    foreach (var s in Bottom)
                    {
                        prod.Sizes.Add(new Size(s));
                    }
                }

                await _productsService.SaveProductAsync(product);
                product.Sizes = productCurrent.Sizes;
                TempData["message"] = $"{product.Name} has been saved";
            }
            var viewModel = new AdminProductViewModel
            {
                Product = product,
                Categories = _productsService.Categories,
                Colors = _productsService.Colors,
                SizesForCreateProduct = _productsService.SizesForCreateProduct
            };
            return View(viewModel);
        }
        [Route("Admin/RemoveImage")]
        public async Task<IActionResult> RemoveImage(int productId, string imageName)
        {
            if (!isProduct(productId))
            {
                return RedirectToAction("Create");
            }

            await _productsService.RemoveImage(productId, imageName);

            return RedirectToAction("Edit", new { productId });
        }
        [Route("Admin/AddImage")]
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

        [Route("Admin/Create")]
        public IActionResult Create(int categoryID)
        {
            var category = _productsService._categoryRepository.Categories.FirstOrDefault(c => c.ID == categoryID);

            if (category == null)
                return NotFound();

            Product product = new Product();

            if (category.Name == "Комплекты")
            {
                //Category bra = _productsService.Categories.FirstOrDefault(p => p.Name == );
                //Category tr = _productsService.Categories.FirstOrDefault(p => p.Name == "Трусики");

                product.Products = new List<Product> { new Product("Top"), new Product("Bottom") };
            }
            product.Category = category;
            var viewModel = new AdminProductViewModel
            {
                Product = product,
                Categories = _productsService.Categories,
                Colors = _productsService.Colors,
                SizesForCreateProduct = _productsService.SizesForCreateProduct
            };
            return View("Edit", viewModel);
        }

        [Route("Admin/CreateCategory")]
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
        [Route("Admin/CreateCategory/{category}")]
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

        [Route("Admin/Delete")]
        public async Task<IActionResult> Delete(int productId)
        {
            Product deletedProduct = await _productsService.DeleteProductAsync(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = $"{deletedProduct.Name} was deleted";
            }
            return RedirectToAction("Index");
        }
        [Route("Admin/DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int categoryID)
        {
            Category deletedCategory = await _productsService._categoryRepository.DeleteCategoryAsync(categoryID);
            if (deletedCategory != null)
            {
                TempData["message"] = $"{deletedCategory.Name} was deleted";
            }
            return RedirectToAction("Categories");
        }

        [Route("Admin/EditCategory")]
        public IActionResult EditCategory(int categoryID)
        {
            var category = _productsService._categoryRepository.Categories.FirstOrDefault(c => c.ID == categoryID);

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
        [Route("Admin/EditCategory")]
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
            var product = _productsService.Products.FirstOrDefault(p => p.Id == productId);
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
