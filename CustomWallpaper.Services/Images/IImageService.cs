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
        Task<IEnumerable<Image>> GetAllImagesAsync();
        Task<Image> GetImageByIdAsync(int id);
        Task<Image> GetImageByHashAsync(string hash);
        Task AddImageAsync(Image image);
        Task<bool> ImageExistsAsync(string hash);
        Task UpdateImageAsync(Image image);

        Task CopyToClipboardAsync(string imagePath);
    }
}
