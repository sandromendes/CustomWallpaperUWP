using CustomWallpaper.Core.Services;
using CustomWallpaper.Core.Utils;
using CustomWallpaper.Domain.Application;
using CustomWallpaper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CustomWallpaper.Services.Images
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _repository;
        private readonly ILoggerService _logger;

        public ImageService(IImageRepository repository,
            ILoggerService logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddOrUpdateFromFileAsync(StorageFile file)
        {
            var hash = await FileHasher.ComputeHashAsync(file);
            if (await _repository.ExistsAsync(hash))
                return;

            var props = await file.Properties.GetImagePropertiesAsync();
            var basicProps = await file.GetBasicPropertiesAsync();

            var image = new Image
            {
                FileName = file.Name,
                FilePath = file.Path,
                FileExtension = file.FileType,
                FileSizeInBytes = (long)basicProps.Size,
                DateCreated = file.DateCreated.ToString("o"),
                DateModified = basicProps.DateModified.ToString("o"),
                Width = (int)props.Width,
                Height = (int)props.Height,
                Hash = hash,
                IsFavorite = false
            };

            await _repository.AddAsync(image);
        }

        public Task<IEnumerable<Image>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Image> GetByHashAsync(string hash) => _repository.GetByHashAsync(hash);

        public Task<IEnumerable<Image>> GetAllImagesAsync() => _repository.GetAllAsync();
        public Task<Image> GetImageByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task<Image> GetImageByHashAsync(string hash) => _repository.GetByHashAsync(hash);
        public Task AddImageAsync(Image image) => _repository.AddAsync(image);
        public Task<bool> ImageExistsAsync(string hash) => _repository.ExistsAsync(hash);
        public Task UpdateImageAsync(Image image) => _repository.UpdateAsync(image);
    }
}
