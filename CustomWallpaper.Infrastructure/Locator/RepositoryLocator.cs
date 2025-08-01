using CustomWallpaper.CrossCutting.Services;
using CustomWallpaper.Domain.Repositories;
using CustomWallpaper.Infrastructure.Repositories;

namespace CustomWallpaper.Infrastructure.Locator
{
    public static class RepositoryLocator
    {
        private static IImageRepository _imageRepository;
        private static ILoggerService _logger;
        private static IWallpaperHistoryRepository _historyRepository;

        public static void Initialize(ILoggerService logger)
        {
            _logger = logger;

            _imageRepository = new ImageRepository(logger);
            _historyRepository = new WallpaperHistoryRepository(logger);
        }

        public static IImageRepository GetImageRepository()
        {
            if (_imageRepository == null)
            {
                _logger = _logger ?? new LoggerService();
                _imageRepository = new ImageRepository(_logger);
            }

            return _imageRepository;
        }

        public static IWallpaperHistoryRepository GetWallpaperHistoryRepository()
        {
            if (_historyRepository == null)
            {
                _logger = _logger ?? new LoggerService();
                _historyRepository = new WallpaperHistoryRepository(_logger);
            }

            return _historyRepository;
        }
    }
}
