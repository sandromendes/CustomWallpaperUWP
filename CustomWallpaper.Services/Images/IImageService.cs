using CustomWallpaper.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace CustomWallpaper.Services.Images
{
    public interface IImageService
    {
        Task AddOrUpdateFromFileAsync(StorageFile file);
        Task<IEnumerable<Image>> GetAllAsync();
        Task<Image> GetByHashAsync(string hash);
        Task<Image> GetByIdAsync(int id);
        Task<bool> ExistsAsync(string hash);
    }
}
