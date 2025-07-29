using CustomWallpaper.Domain.Application;
using CustomWallpaper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CustomWallpaper.Services.Folders
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _repository;

        public FolderService(IFolderRepository repository)
        {
            _repository = repository;
        }

        public async Task AddFolderAsync(string path, string token)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (await _repository.ExistsAsync(path))
                return;

            var folder = new Folder
            {
                FolderPath = path,
                FolderName = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar)),
                DateAdded = DateTime.UtcNow,
                AccessToken = token
            };

            await _repository.AddAsync(folder);
        }

        public Task<IEnumerable<Folder>> GetAllFoldersAsync() => _repository.GetAllAsync();

        public Task<bool> FolderExistsAsync(string path) => _repository.ExistsAsync(path);

        public Task RemoveFolderAsync(int id) => _repository.RemoveAsync(id);
    }
}
