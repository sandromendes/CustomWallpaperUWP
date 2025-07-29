using CustomWallpaper.Services.BackgroundTasks;
using CustomWallpaper.Tasks;
using System.Threading.Tasks;
using System;
using Windows.ApplicationModel.Background;
using CustomWallpaper.CrossCutting.Services;

public class BackgroundTaskService : IBackgroundTaskService
{
    private readonly ILoggerService _logger;

    public BackgroundTaskService(ILoggerService logger)
    {
        _logger = logger;
    }

    public async Task RegisterStressTaskAsync()
    {
        try
        {
            const string taskName = nameof(DatabaseStressTask);

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                _logger.Info(nameof(RegisterStressTaskAsync), $"Existing Task: {task.Key} | Name: {task.Value.Name}");
            }

            UnregisterTaskAsync(taskName);

            var trigger = new ApplicationTrigger();
            var builder = new BackgroundTaskBuilder
            {
                Name = taskName,
                TaskEntryPoint = typeof(DatabaseStressTask).FullName
            };

            builder.SetTrigger(trigger);
            var reg = builder.Register();

            await trigger.RequestAsync();

            _logger.Info(nameof(RegisterStressTaskAsync), $"{taskName} registered.");
        }
        catch (Exception ex)
        {
            _logger.Error(nameof(RegisterStressTaskAsync), ex, "Failed to register stress task.");
        }
    }

    public Task UnregisterStressTaskAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                UnregisterTaskAsync(nameof(DatabaseStressTask));
                _logger.Info(nameof(UnregisterStressTaskAsync), "Stress task unregistered.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(UnregisterStressTaskAsync), ex, "Failed to unregister stress task.");
            }
        });
    }

    public async Task RegisterSmartEngineTaskAsync()
    {
        try
        {
            const string taskName = nameof(SmartEngineTask);

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                _logger.Info(nameof(RegisterSmartEngineTaskAsync), $"Existing Task: {task.Key} | Name: {task.Value.Name}");
            }

            UnregisterTaskAsync(taskName);

            var trigger = new ApplicationTrigger();
            var builder = new BackgroundTaskBuilder
            {
                Name = taskName,
                TaskEntryPoint = typeof(SmartEngineTask).FullName
            };

            builder.SetTrigger(trigger);
            builder.Register();

            await trigger.RequestAsync();

            _logger.Info(nameof(RegisterSmartEngineTaskAsync), $"{taskName} registered.");
        }
        catch (Exception ex)
        {
            _logger.Error(nameof(RegisterSmartEngineTaskAsync), ex, "Failed to register SmartEngine task.");
        }
    }

    public Task UnregisterSmartEngineTaskAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                UnregisterTaskAsync(nameof(SmartEngineTask));
                _logger.Info(nameof(UnregisterSmartEngineTaskAsync), "SmartEngine task unregistered.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(UnregisterSmartEngineTaskAsync), ex, "Failed to unregister SmartEngine task.");
            }
        });
    }

    private async void UnregisterTaskAsync(string name)
    {
        foreach (var task in BackgroundTaskRegistration.AllTasks)
        {
            if (task.Value.Name == name)
            {
                task.Value.Unregister(true);
                _logger.Info(nameof(UnregisterTaskAsync), $"Task {name} removed.");
            }
        }
    }
}
