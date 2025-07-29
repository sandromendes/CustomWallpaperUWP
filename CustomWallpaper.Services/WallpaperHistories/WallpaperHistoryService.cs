using CustomWallpaper.CrossCutting.Services;
using CustomWallpaper.Domain.Application;
using CustomWallpaper.Domain.Entities;
using CustomWallpaper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Services.WallpaperHistories
{
    public class WallpaperHistoryService : IWallpaperHistoryService
    {
        private readonly IWallpaperHistoryRepository _repository;
        private readonly ILoggerService _logger;

        public WallpaperHistoryService(IWallpaperHistoryRepository repository, ILoggerService logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddAsync(string imageId, string source)
        {
            try
            {
                var history = new WallpaperHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    ImageId = imageId,
                    AppliedAt = DateTime.UtcNow.ToString("o"),
                    Source = source
                };

                _logger.Info(nameof(WallpaperHistoryService), $"Adding new history entry: ImageId={imageId}, Source={source}");

                await _repository.AddAsync(history);

                _logger.Info(nameof(WallpaperHistoryService), "History entry added successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryService), ex, $"Failed to add history entry. Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<WallpaperHistory>> GetAllAsync()
        {
            try
            {
                _logger.Info(nameof(WallpaperHistoryService), "Fetching all history entries.");
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryService), ex, "Failed to fetch all history entries.");
                return Array.Empty<WallpaperHistory>();
            }
        }

        public async Task<WallpaperHistory> GetLastAppliedAsync()
        {
            try
            {
                _logger.Info(nameof(WallpaperHistoryService), "Fetching last applied wallpaper history.");
                return await _repository.GetLastAppliedAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryService), ex, "Failed to fetch last applied entry.");
                return null;
            }
        }

        public async Task<IEnumerable<WallpaperHistoryDto>> GetHistoryWithImageNamesAsync()
        {
            try
            {
                _logger.Info(nameof(WallpaperHistoryService), "Fetching history entries with image names.");
                return await _repository.GetAllWithImageNameAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryService), ex, "Failed to fetch history with image names.");
                return Array.Empty<WallpaperHistoryDto>();
            }
        }

        public async Task AddAsync(WallpaperHistory history)
        {
            try
            {
                _logger.Info(nameof(WallpaperHistoryService), $"Adding custom history entry for ImageId={history.ImageId}, Source={history.Source}");

                await _repository.AddAsync(history);

                _logger.Info(nameof(WallpaperHistoryService), "Custom history entry added successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryService), ex, $"Failed to add custom history entry. Error: {ex.Message}");
            }
        }
    }
}
