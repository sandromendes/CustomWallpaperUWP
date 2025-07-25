using System.Threading.Tasks;

namespace CustomWallpaper.Services.BackgroundTasks
{
    public interface IBackgroundTaskService
    {
        Task RegisterSmartEngineTaskAsync();
        Task UnregisterSmartEngineTaskAsync();
        Task RegisterStressTaskAsync();
        Task UnregisterStressTaskAsync();
    }
}
