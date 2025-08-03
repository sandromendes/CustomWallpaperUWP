using System.Threading.Tasks;
using Windows.Storage;

namespace CustomWallpaper.Infrastructure.Services
{
    public interface IWallpaperService
    {
        Task SetWallpaperAsync(StorageFile file);
        Task SetLockscreenAsync(StorageFile file);
    }
}
