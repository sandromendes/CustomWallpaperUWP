using CustomWallpaper.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Domain.Services
{
    public interface IFolderService
    {
        Task<IEnumerable<string>> RegisterAllFoldersRecursivelyAsync(string rootFolder, string token);
        Task AddFolderAsync(string path, string token);
        Task<IEnumerable<Folder>> GetAllFoldersAsync();
        Task<bool> FolderExistsAsync(string path);
        Task RemoveFolderAsync(int id);
    }
}
