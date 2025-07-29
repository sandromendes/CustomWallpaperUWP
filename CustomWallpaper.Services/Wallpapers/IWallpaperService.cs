using System.Threading.Tasks;
using Windows.Storage;

namespace CustomWallpaper.Services.Wallpapers
{
    public interface IWallpaperService
    {
        Task SetWallpaperAsync(StorageFile file);
        Task SetLockscreenAsync(StorageFile file);
    }
}
