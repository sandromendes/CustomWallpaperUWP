using CustomWallpaper.Domain.Application;
using CustomWallpaper.Infrastructure.Repositories;

namespace CustomWallpaper.Core.Locator
{
    namespace CustomWallpaper.Infrastructure
    {
        public static class RepositoryLocator
        {
            private static IImageRepository _imageRepository;
            private static IWallpaperHistoryRepository _historyRepository;

            public static void Initialize()
            {
                _imageRepository = new ImageRepository();
                _historyRepository = new WallpaperHistoryRepository();
            }

            public static IImageRepository GetImageRepository()
            {
                if (_imageRepository == null)
                    _imageRepository = new ImageRepository();

                return _imageRepository;
            }

            public static IWallpaperHistoryRepository GetWallpaperHistoryRepository()
            {
                if (_historyRepository == null)
                    _historyRepository = new WallpaperHistoryRepository();

                return _historyRepository;
            }
        }
    }
}
