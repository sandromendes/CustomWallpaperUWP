using CustomWallpaper.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Domain.Application
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetAllAsync();
        Task<Image> GetByIdAsync(int id);
        Task<Image> GetByHashAsync(string hash);
        Task AddAsync(Image image);
        Task<bool> ExistsAsync(string hash);
        Task UpdateAsync(Image image);
    }
}
