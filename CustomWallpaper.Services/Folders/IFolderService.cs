using CustomWallpaper.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Services.Folders
{
    public interface IFolderService
    {
        Task AddFolderAsync(string path, string token);
        Task<IEnumerable<Folder>> GetAllFoldersAsync();
        Task<bool> FolderExistsAsync(string path);
        Task RemoveFolderAsync(int id);
    }
}
