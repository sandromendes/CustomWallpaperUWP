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

        public WallpaperHistoryService(IWallpaperHistoryRepository repository)
        {
            _repository = repository;
        }

        public Task AddAsync(int imageId, string source)
        {
            return _repository.AddAsync(new WallpaperHistory
            {
                ImageId = imageId,
                AppliedAt = DateTime.UtcNow.ToString("o"),
                Source = source
            });
        }

        public Task<IEnumerable<WallpaperHistory>> GetAllAsync() => _repository.GetAllAsync();
        public Task<WallpaperHistory> GetLastAppliedAsync() => _repository.GetLastAppliedAsync();
        public Task<IEnumerable<WallpaperHistoryDto>> GetHistoryWithImageNamesAsync() => _repository.GetAllWithImageNameAsync();
        public Task AddAsync(WallpaperHistory history) => _repository.AddAsync(history);
    }
}
