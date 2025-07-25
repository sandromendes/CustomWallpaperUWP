using CustomWallpaper.Core.Locator;
using CustomWallpaper.Core.Locator.CustomWallpaper.Infrastructure;
using CustomWallpaper.Domain.Application;
using CustomWallpaper.Tasks.Logs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace CustomWallpaper.Tasks
{
    public sealed class DatabaseStressTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private IImageRepository _imageRepository;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnCanceled;

            await BackgroundTaskLoggerHelper.InfoAsync(nameof(DatabaseStressTask), "Task STARTED");

            try
            {
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

                    await BackgroundTaskLoggerHelper.InfoAsync(nameof(DatabaseStressTask),
                        $"Updated {images.Count()} images at {DateTime.UtcNow}");

                    await Task.Delay(1000);
                }

                await BackgroundTaskLoggerHelper.InfoAsync(nameof(DatabaseStressTask), "Task FINISHED 6-minute cycle");
            }
            catch (Exception ex)
            {
                await BackgroundTaskLoggerHelper.ErrorAsync(nameof(DatabaseStressTask), ex);
            }
            finally
            {
                _deferral.Complete();
            }
        }

        private async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            await BackgroundTaskLoggerHelper.InfoAsync(nameof(DatabaseStressTask), $"Task CANCELED. Reason: {reason}");
        }
    }
}
