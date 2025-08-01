using CustomWallpaper.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace CustomWallpaper.Domain.Services
{
    public interface IImageService
    {
        Task<IEnumerable<Image>> GetAllAsync();
        Task<Image> GetByIdAsync(int id);
        Task<Image> GetByHashAsync(string hash);
        Task<Image> GetByPathAsync(string path);
        Task AddOrUpdateFromFileAsync(StorageFile file);
        Task<bool> ExistsAsync(string hash);
    }
}
