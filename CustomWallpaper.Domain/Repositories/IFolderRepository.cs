using CustomWallpaper.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Domain.Repositories
{
    public interface IFolderRepository
    {
        Task AddAsync(Folder folder);
        Task<IEnumerable<Folder>> GetAllAsync();
        Task<bool> ExistsAsync(string folderPath);
        Task RemoveAsync(int id);
    }
}
