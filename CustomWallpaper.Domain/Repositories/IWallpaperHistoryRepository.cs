using CustomWallpaper.Domain.Entities;
using CustomWallpaper.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Domain.Repositories
{
    public interface IWallpaperHistoryRepository
    {
        Task AddAsync(WallpaperHistory history);
        Task<IEnumerable<WallpaperHistory>> GetAllAsync();
        Task<IEnumerable<WallpaperHistoryDto>> GetAllWithImageNameAsync();
        Task<WallpaperHistory> GetLastAppliedAsync();
    }
}
