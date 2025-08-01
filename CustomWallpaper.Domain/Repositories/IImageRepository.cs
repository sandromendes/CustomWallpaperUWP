using CustomWallpaper.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Domain.Repositories
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetAllAsync();
        Task<Image> GetByIdAsync(int id);
        Task<Image> GetByHashAsync(string hash);
        Task<Image> GetByPathAsync(string path);
        Task AddAsync(Image image);
        Task UpdateAsync(Image image);
        Task<bool> ExistsAsync(string hash);
    }
}
