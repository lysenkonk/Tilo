using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tilo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
namespace Tilo.Services
{
    public class ProductsService
    {
        private const string BigFilesFolder = "/Files/Bg/";
        private const string SmallFilesFolder = "/Files/Sm/";

        public readonly IProductRepository _repository;
        public readonly ICategoryRepository _categoryRepository;
        private readonly IHostingEnvironment _appEnvironment;

        public ProductsService(IProductRepository repo, ICategoryRepository categoryRepo, IHostingEnvironment appEnvironment)
        {
            _repository = repo;
            _categoryRepository = categoryRepo;
            _appEnvironment = appEnvironment;
        }

        public IEnumerable<Product> Products => _repository.Products.Include(p => p.Images).Include(p => p.Category).Include(p => p.Products).ThenInclude(subProduct => subProduct.Sizes).Include(p => p.Sizes);
        public IEnumerable<Category> Categories => _categoryRepository.Categories;
        public IEnumerable<Category> ParentCategories => _repository.Categories.Where(p => p.ParentCategory == null);

        public IEnumerable<string> Colors => new string[] { "чёрный", "белый", "красный", "зелёный", "синий", "айвори", "марсала", "оранжевый", "розовый", "желтый" };
        public List<Models.Size> SizesForCreateProduct = new List<Models.Size> {  new Models.Size("70"), new Models.Size("70"), new Models.Size("80") , new Models.Size("A"), new Models.Size("B"), new Models.Size("C"), new Models.Size("D"), new Models.Size("E"), new Models.Size("XS"), new Models.Size("S"), new Models.Size("M"), new Models.Size("L"), new Models.Size("One size") };

        public async Task<Product> SaveProductAsync(Product product)
        {
            return await _repository.SaveProductAsync(product);
        }
        public async Task<Category> SaveCategoryAsync(string category, string parentCategory = null)
        {
            return await _categoryRepository.AddCategoryAsync(category, parentCategory);
        }

        public async Task<Product> DeleteProductAsync(int productID)
        {
            var product = await _repository.Products.FirstOrDefaultAsync(p => p.Id == productID);

            if (product == null)
                throw new Exception("404 Not Found Product"); // TODO make proper hadling
            List<FileModel> images = new List<FileModel>(product.Images);
            foreach (var image in images)
            {
                RemoveImageFiles(image.Name);
                await _repository.RemoveImageAsync(product.Id, image.Name);
            }

            return await _repository.DeleteProductAsync(productID);
        }

        public async Task AddImage(int productId, IFormFile uploadedFile)
        {
            Product product = await _repository.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new Exception("404 Not Found"); // TODO make proper hadling

            FileModel file = null;
            if (uploadedFile != null)
            {
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + BigFilesFolder + uploadedFile.FileName, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                Bitmap resized = ResizeImage(uploadedFile.OpenReadStream(), 195, 195);
                resized.Save(_appEnvironment.WebRootPath + SmallFilesFolder + uploadedFile.FileName, ImageFormat.Png);
                file = new FileModel { Name = uploadedFile.FileName };
            }

            await _repository.AddImageAsync(product.Id, file);
        }

        public async Task RemoveImage(int productId, string imageName)
        {
            await _repository.RemoveImageAsync(productId, imageName);
            RemoveImageFiles(imageName);
        }

        private static Bitmap ResizeImage(Stream stream, int width, int height)
        {
            var resized = new Bitmap(width, height);
            using (var image = new Bitmap(stream))
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, width, height);
            }

            return resized;
        }

        private void RemoveImageFiles(string imageName)
        {
            if (File.Exists(_appEnvironment.WebRootPath + BigFilesFolder + imageName))
                File.Delete(_appEnvironment.WebRootPath + BigFilesFolder + imageName);
            else
                return; // TODO make proper hadling

            if (File.Exists(_appEnvironment.WebRootPath + SmallFilesFolder + imageName))
                File.Delete(_appEnvironment.WebRootPath + SmallFilesFolder + imageName);
            else
                return; // TODO make proper hadling
        }

        public async Task<Product> RemoveSizes(long productId, List<string> sizes)
        {     
            return await _repository.RemoveSizeAsync(productId, sizes);
        }
    }
}
