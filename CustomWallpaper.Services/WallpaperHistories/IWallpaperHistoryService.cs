using CustomWallpaper.Domain.Entities;
using CustomWallpaper.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Services.WallpaperHistories
{
    public interface IWallpaperHistoryService
    {
        Task AddAsync(string imageId, string source);
        Task<IEnumerable<WallpaperHistory>> GetAllAsync();
        Task<WallpaperHistory> GetLastAppliedAsync();
        Task<IEnumerable<WallpaperHistoryDto>> GetHistoryWithImageNamesAsync();
        Task AddAsync(WallpaperHistory history);
    }
}
