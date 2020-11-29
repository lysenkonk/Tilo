using System.IO;
using Tilo.Models;
using Tilo.Models.ViewModels;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Tilo.Services
{
    public class PhotosService
    {
        private const string BigGalleryFolder = "/Gallery/Bg/";
        private const string SmallGalleryFolder = "/Gallery/Sm/";

        public readonly IFileModelRepository _repository;
        private readonly IHostingEnvironment _appEnvironment;

        public PhotosService(IFileModelRepository repo, IHostingEnvironment appEnvironment)
        {
            _repository = repo;
            _appEnvironment = appEnvironment;
        }

        public List<FileModel> Photos => new List<FileModel>(_repository.FileModels);

        public async Task<FileModel> SavePhoto(IFormFile uploadedFile)
        {
            FileModel photo = null;
            Image image = Image.FromStream(uploadedFile.OpenReadStream(), true, true);
            if (uploadedFile != null)
            {
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + BigGalleryFolder + uploadedFile.FileName, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                
                Bitmap resized = ResizePhoto(uploadedFile.OpenReadStream(), image.Width / 3, image.Height / 3);
                resized.Save(_appEnvironment.WebRootPath + SmallGalleryFolder + uploadedFile.FileName, ImageFormat.Png);
                photo = new FileModel { Name = uploadedFile.FileName };
            }
            return await _repository.SavePhotoModelAsync(photo);
        }

        public async Task RemovePhoto(string photoName)
        {
            await _repository.RemovePhotoModelAsync(photoName);
            RemovePhotoFiles(photoName);
        }

        private static Bitmap ResizePhoto(Stream stream, int width, int height)
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

        private void RemovePhotoFiles(string photoName)
        {
            if (File.Exists(_appEnvironment.WebRootPath + BigGalleryFolder + photoName))
                File.Delete(_appEnvironment.WebRootPath + BigGalleryFolder + photoName);
            else
                return;

            if (File.Exists(_appEnvironment.WebRootPath + SmallGalleryFolder + photoName))
                File.Delete(_appEnvironment.WebRootPath + SmallGalleryFolder + photoName);
            else
                return;
        }
    }
}
