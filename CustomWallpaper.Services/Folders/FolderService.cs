using CustomWallpaper.CrossCutting.Services;
using CustomWallpaper.Domain.Repositories;
using CustomWallpaper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CustomWallpaper.Domain.Services;

namespace CustomWallpaper.Services.Folders
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _repository;
        private readonly ILoggerService _logger;

        public FolderService(IFolderRepository repository, ILoggerService logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddFolderAsync(string path, string token)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.Info(nameof(FolderService), "Attempted to add a folder with an empty or null path.");
                return;
            }

            try
            {
                if (await _repository.ExistsAsync(path))
                {
                    _logger.Info(nameof(FolderService), $"Folder already registered: {path}");
                    return;
                }

                var folder = new Folder
                {
                    FolderPath = path,
                    FolderName = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar)),
                    DateAdded = DateTime.UtcNow,
                    AccessToken = token
                };

                await _repository.AddAsync(folder);

                _logger.Info(nameof(FolderService), $"Folder successfully added: {folder.FolderName} ({folder.FolderPath})");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(FolderService), ex, $"Error while adding folder: {path}");
                throw;
            }
        }

        public async Task<IEnumerable<Folder>> GetAllFoldersAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(FolderService), ex, "Failed to retrieve all folders.");
                throw;
            }
        }

        public async Task<bool> FolderExistsAsync(string path)
        {
            try
            {
                return await _repository.ExistsAsync(path);
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(FolderService), ex, $"Error checking if folder exists: {path}");
                throw;
            }
        }

        public async Task RemoveFolderAsync(int id)
        {
            try
            {
                await _repository.RemoveAsync(id);
                _logger.Info(nameof(FolderService), $"Folder removed (Id: {id})");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(FolderService), ex, $"Error removing folder with Id: {id}");
                throw;
            }
        }
    }
}
