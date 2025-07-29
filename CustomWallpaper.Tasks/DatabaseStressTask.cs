using CustomWallpaper.CrossCutting.Services;
using CustomWallpaper.Domain.Application;
using CustomWallpaper.Infrastructure.Locator;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace CustomWallpaper.Tasks
{
    public sealed class DatabaseStressTask : IBackgroundTask
    {
        private const string LogFilePrefix = "CustomWallpaperBgTask";

        private BackgroundTaskDeferral _deferral;
        private IImageRepository _imageRepository;

        private ILoggerService _loggerService;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnCanceled;

            _loggerService = new LoggerService();

            try
            {
                _loggerService.LogFileName = LogFilePrefix;

                RepositoryLocator.Initialize(_loggerService);

                _loggerService.Info(nameof(DatabaseStressTask), "Task STARTED");

                _imageRepository = RepositoryLocator.GetImageRepository();

                var startTime = DateTime.UtcNow;
                var endTime = startTime.AddMinutes(6);

                while (DateTime.UtcNow < endTime)
                {
                    var images = await _imageRepository.GetAllAsync();

                    foreach (var image in images)
                    {
                        image.DateModified = DateTime.UtcNow.ToString("o");
                        await _imageRepository.UpdateAsync(image);
                    }

                    _loggerService.Info(nameof(DatabaseStressTask),
                        $"Updated {images.Count()} images at {DateTime.UtcNow}");

                    await Task.Delay(1000);
                }

                _loggerService.Info(nameof(DatabaseStressTask), "Task FINISHED 6-minute cycle");
            }
            catch (Exception ex)
            {
                _loggerService.Error(nameof(DatabaseStressTask), ex);
            }
            finally
            {
                _deferral.Complete();
            }
        }

        private async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _loggerService.Info(nameof(DatabaseStressTask), $"Task CANCELED. Reason: {reason}");
        }
    }
}
