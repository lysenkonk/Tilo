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
using System.Net.Http;
using System;

namespace Tilo.Services
{
    public class PhotosService
    {
        private const string BigGalleryFolder = "/Files/Bg/";
        private const string SmallGalleryFolder = "/Files/Sm/";
        private const string SmallGalleryFolder2 = "/Files/Sm2/";

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
                double k = (double)image.Width / 190;
                int height = (int)((double)image.Height / k);
                Bitmap resized = ResizePhoto(uploadedFile.OpenReadStream(), 190, height);
                resized.Save(_appEnvironment.WebRootPath + SmallGalleryFolder + uploadedFile.FileName, ImageFormat.Png);
                photo = new FileModel { Name = uploadedFile.FileName };
            }
            return await _repository.SavePhotoModelAsync(photo);
        }
        public async Task<FileModel> SavePhotoAlbumOrient(IFormFile uploadedFile)
        {
            FileModel photo = null;
            Image image = Image.FromStream(uploadedFile.OpenReadStream(), true, true);
            if (uploadedFile != null)
            {
                //using (var fileStream = new FileStream(_appEnvironment.WebRootPath + BigGalleryFolder + uploadedFile.FileName, FileMode.Create))
                //{
                    //await uploadedFile.CopyToAsync(fileStream);
                    Bitmap resizedB = ResizePhotoAlbumOrient(uploadedFile.OpenReadStream());
                    resizedB.Save(_appEnvironment.WebRootPath + BigGalleryFolder + uploadedFile.FileName, ImageFormat.Png);
                //}
                photo = new FileModel { Name = uploadedFile.FileName };

                //double k = (double)image.Width / 190;
                //int height = (int)((double)image.Height / k);
                //Bitmap resized = ResizePhoto(uploadedFile.OpenReadStream(), 190, height);
                //resized.Save(_appEnvironment.WebRootPath + SmallGalleryFolder + uploadedFile.FileName, ImageFormat.Png);
                //photo = new FileModel { Name = uploadedFile.FileName };
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
        private static Bitmap ResizePhotoAlbumOrient(Stream stream)
        {

            using (var sourceImage = new Bitmap(stream))
            {
                int targetWidth = sourceImage.Width/2;
                int targetHeight = sourceImage.Height;
                int x = sourceImage.Width / 2 - targetWidth / 2;
                int y = sourceImage.Height / 2;
                Rectangle cropArea = new Rectangle(x, y, targetWidth, targetHeight);

                var imageCut = sourceImage.Clone(cropArea, sourceImage.PixelFormat);
                var resized = new Bitmap(targetWidth, targetHeight);

                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(imageCut, 0, 0, targetWidth, targetHeight);
                }
                return resized;
            }
        }
        public async void AddThumbnailPhotoToNewFolderPhoto(string fileName)
        {
            try
            {
                string filePath = _appEnvironment.WebRootPath + BigGalleryFolder + fileName;
                string filePathSm2 = _appEnvironment.WebRootPath + SmallGalleryFolder2 + fileName;
                using (MemoryStream ms = new MemoryStream())
                {
                    if (!File.Exists(filePathSm2))
                    {
                        using (var fileSteam = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                        {
                           await fileSteam.CopyToAsync(ms);
                            Image image = Image.FromStream(ms, true, true);
                            double k = (double)image.Width / 190;
                            int height = (int)((double)image.Height / k);
                            Bitmap resized = ResizePhoto(fileSteam, 190, height);
                            resized.Save(_appEnvironment.WebRootPath + SmallGalleryFolder2 + fileName, ImageFormat.Png);
                            ms.Close();
                        }
                    }
                }
            }
            catch (IOException e)
            {
                return ;
            }
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
