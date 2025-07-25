using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace CustomWallpaper.Tasks.Logs
{
    internal static class BackgroundTaskLoggerHelper
    {
        private const string LogFilePrefix = "CustomWallpaperBgTask";

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private static string GetLogFileName()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return $"{LogFilePrefix}_{date}.log";
        }

        internal static async Task InfoAsync(string source, string message)
        {
            await WriteLogAsync("INFO", source, message);
        }

        internal static async Task ErrorAsync(string source, Exception ex)
        {
            string fullMessage = $"Exception occurred\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
            await WriteLogAsync("ERROR", source, fullMessage);
        }

        internal static async Task ErrorAsync(string source, Exception ex, string customMessage)
        {
            string fullMessage = $"{customMessage}\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
            await WriteLogAsync("ERROR", source, fullMessage);
        }

        private static async Task WriteLogAsync(string level, string source, string message)
        {
            await _semaphore.WaitAsync();
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(GetLogFileName(), CreationCollisionOption.OpenIfExists);

                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {source}: {message}{Environment.NewLine}";
                await FileIO.AppendTextAsync(file, logEntry);
            }
            catch
            {
                // Silencia falhas de log para não interferir na execução da task
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
