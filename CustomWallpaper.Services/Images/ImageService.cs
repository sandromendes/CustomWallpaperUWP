using CustomWallpaper.CrossCutting.Services;
using CustomWallpaper.CrossCutting.Utils;
using CustomWallpaper.Domain.Repositories;
using CustomWallpaper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using CustomWallpaper.Domain.Services;
using System.IO;
using System.Linq;
using Windows.Storage.AccessCache;

namespace CustomWallpaper.Services.Images
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _repository;
        private readonly ILoggerService _logger;
        private const string Source = nameof(ImageService);

        public ImageService(IImageRepository repository, ILoggerService logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task RegisterImagesInFoldersAsync(IEnumerable<string> folders, string token)
        {
            foreach (var folder in folders)
            {
                var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);

                var imagePaths = await GetImagesInFolderAsync(storageFolder);

                foreach (var imagePath in imagePaths)
                    await RegisterImageAsync(imagePath, folder);
            }
        }

        private async Task<IEnumerable<string>> GetImagesInFolderAsync(StorageFolder folder)
        {
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

            var files = await folder.GetFilesAsync();
            return files
                .Where(file => supportedExtensions.Contains(Path.GetExtension(file.Name).ToLowerInvariant()))
                .Select(file => file.Path);
        }


        private async Task RegisterImageAsync(string imagePath, string folderPath)
        {
            var file = await StorageFile.GetFileFromPathAsync(imagePath);

            _logger.Info(Source, $"Starting file processing: {file?.Name}");

            var hash = await FileHasher.ComputeHashAsync(file);
            _logger.Info(Source, $"Computed hash: {hash}");

            if (await _repository.ExistsAsync(hash))
            {
                _logger.Info(Source, $"Image with hash {hash} already exists. Skipping.");
                return;
            }

            var props = await file.Properties.GetImagePropertiesAsync();
            var basicProps = await file.GetBasicPropertiesAsync();

            var image = new Image
            {
                FileName = file.Name,
                FilePath = file.Path,
                FileExtension = file.FileType,
                FileSizeInBytes = (long)basicProps.Size,
                DateCreated = file.DateCreated.DateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                DateModified = basicProps.DateModified.DateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                Width = (int)props.Width,
                Height = (int)props.Height,
                Hash = hash,
                IsFavorite = false
            };

            await _repository.AddAsync(image);
        }

        public async Task AddOrUpdateFromFileAsync(StorageFile file)
        {
            try
            {
                _logger.Info(Source, $"Starting file processing: {file?.Name}");

                var hash = await FileHasher.ComputeHashAsync(file);
                _logger.Info(Source, $"Computed hash: {hash}");

                if (await _repository.ExistsAsync(hash))
                {
                    _logger.Info(Source, $"Image with hash {hash} already exists. Skipping.");
                    return;
                }

                var props = await file.Properties.GetImagePropertiesAsync();
                var basicProps = await file.GetBasicPropertiesAsync();

                var image = new Image
                {
                    FileName = file.Name,
                    FilePath = file.Path,
                    FileExtension = file.FileType,
                    FileSizeInBytes = (long)basicProps.Size,
                    DateCreated = file.DateCreated.DateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    DateModified = basicProps.DateModified.DateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    Width = (int)props.Width,
                    Height = (int)props.Height,
                    Hash = hash,
                    IsFavorite = false
                };

                await _repository.AddAsync(image);
                _logger.Info(Source, $"Image added successfully: {file.Name}");
            }
            catch (Exception ex)
            {
                _logger.Error(Source, ex, $"Error while adding or updating image from file {file?.Name}");
            }
        }

        public async Task<IEnumerable<Image>> GetAllAsync()
        {
            try
            {
                _logger.Info(Source, "Retrieving all images.");
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.Fatal(Source, ex, "Fatal error while retrieving all images.");
                return new List<Image>();
            }
        }

        public async Task<Image> GetByHashAsync(string hash)
        {
            try
            {
                _logger.Info(Source, $"Fetching image by hash: {hash}");
                return await _repository.GetByHashAsync(hash);
            }
            catch (Exception ex)
            {
                _logger.Error(Source, ex, $"Error while fetching image with hash {hash}");
                return null;
            }
        }

        public async Task<Image> GetByPathAsync(string path)
        {
            try
            {
                _logger.Info(Source, $"Fetching image by path: {path}");
                return await _repository.GetByPathAsync(path);
            }
            catch (Exception ex)
            {
                _logger.Error(Source, ex, $"Error while fetching image with path {path}");
                return null;
            }
        }

        public async Task<Image> GetByIdAsync(int id)
        {
            try
            {
                _logger.Info(Source, $"Fetching image by ID: {id}");
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.Error(Source, ex, $"Error while fetching image with ID {id}");
                return null;
            }
        }

        public async Task<bool> ExistsAsync(string hash)
        {
            try
            {
                _logger.Info(Source, $"Checking if image exists with hash: {hash}");
                return await _repository.ExistsAsync(hash);
            }
            catch (Exception ex)
            {
                _logger.Error(Source, ex, $"Error while checking existence of image with hash {hash}");
                return false;
            }
        }
    }
}
