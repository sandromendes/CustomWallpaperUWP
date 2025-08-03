using CustomWallpaper.CrossCutting.Services;
using CustomWallpaper.Infrastructure.Services;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

namespace CustomWallpaper.Services.Wallpapers
{
    public class WallpaperService : IWallpaperService
    {
        private const string TempFolderName = "TempBackgroundPhotos";
        private readonly ILoggerService _logger;

        public WallpaperService(ILoggerService logger)
        {
            _logger = logger;
        }

        public async Task SetWallpaperAsync(StorageFile file)
        {
            try
            {
                if (file == null)
                {
                    _logger.Error(nameof(WallpaperService), new ArgumentNullException(nameof(file)), "No file provided to set as wallpaper.");
                    return;
                }

                _logger.Info(nameof(WallpaperService), $"Starting wallpaper setting for file: {file.Path}");

                var success = await SetWallpaperOrLockScreenAsync(file, isLockScreen: false);

                if (success)
                    _logger.Info(nameof(WallpaperService), "Wallpaper successfully set.");
                else
                    _logger.Error(nameof(WallpaperService), new Exception("API returned false."), "Failed to set wallpaper.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperService), ex, "Unexpected error while setting wallpaper.");
            }
        }

        public async Task SetLockscreenAsync(StorageFile file)
        {
            try
            {
                if (file == null)
                {
                    _logger.Error(nameof(WallpaperService), new ArgumentNullException(nameof(file)), "No file provided to set as lockscreen.");
                    return;
                }

                _logger.Info(nameof(WallpaperService), $"Starting lockscreen setting for file: {file.Path}");

                var success = await SetWallpaperOrLockScreenAsync(file, isLockScreen: true);

                if (success)
                    _logger.Info(nameof(WallpaperService), "lockscreen successfully set.");
                else
                    _logger.Error(nameof(WallpaperService), new Exception("API returned false."), "Failed to set lockscreen.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperService), ex, "Unexpected error while setting lockscreen.");
            }
        }

        private async Task<bool> SetWallpaperOrLockScreenAsync(StorageFile file, bool isLockScreen)
        {
            bool success = false;

            if (!UserProfilePersonalizationSettings.IsSupported())
            {
                _logger.Error(nameof(WallpaperService), new NotSupportedException("Personalization not supported."), "UserProfilePersonalizationSettings.IsSupported() returned false.");
                return false;
            }

            try
            {
                await DeleteTempFolderToSetBackgroundAsync();

                var profileSettings = UserProfilePersonalizationSettings.Current;
                var tempFile = await GetTempFileToSetBackground(file);

                if (tempFile == null)
                {
                    _logger.Error(nameof(WallpaperService), new Exception("tempFile is null"), "Failed to create temporary image copy.");
                    return false;
                }

                success = isLockScreen
                    ? await profileSettings.TrySetLockScreenImageAsync(tempFile)
                    : await profileSettings.TrySetWallpaperImageAsync(tempFile);

                _logger.Info(nameof(WallpaperService), $"TrySet{(isLockScreen ? "LockScreen" : "Wallpaper")}ImageAsync returned: {success}");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperService), ex, "Error during SetWallpaperOrLockScreenAsync execution.");
            }

            return success;
        }

        private async Task DeleteTempFolderToSetBackgroundAsync()
        {
            try
            {
                var tempFolder = ApplicationData.Current.LocalFolder;
                var folder = await tempFolder.GetFolderAsync(TempFolderName);
                await folder.DeleteAsync();

                _logger.Info(nameof(WallpaperService), "Temporary folder deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Info(nameof(WallpaperService), $"Temporary folder not found or could not be deleted: {ex.Message}");
            }
        }

        private async Task<StorageFile> GetTempFileToSetBackground(StorageFile fileToBeCopied)
        {
            try
            {
                var tempFolder = ApplicationData.Current.LocalFolder;
                var backgroundFolder = await tempFolder.CreateFolderAsync(
                    TempFolderName,
                    CreationCollisionOption.OpenIfExists);

                var tempFile = await fileToBeCopied.CopyAsync(
                    backgroundFolder,
                    fileToBeCopied.Name,
                    NameCollisionOption.GenerateUniqueName);

                _logger.Info(nameof(WallpaperService), $"Temporary image created: {tempFile.Path}");
                return tempFile;
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperService), ex, "Error while copying image to temporary folder.");
                return null;
            }
        }
    }
}
