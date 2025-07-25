using System;
using System.Diagnostics;
using Windows.Storage;

namespace CustomWallpaper.Core.Services
{
    public class LoggerService : ILoggerService
    {
        private const string LogFileName = "CustomWallpaper";

        public void Info(string source, string message)
        {
            WriteLogAsync("INFO", source, message);
        }

        public void Error(string source, Exception exception, string message = null)
        {
            var fullMessage = $"{message ?? ""}\nException: {exception?.Message}\nStackTrace: {exception?.StackTrace}";
            WriteLogAsync("ERROR", source, fullMessage);
        }

        public void Fatal(string source, Exception exception, string message = null)
        {
            var fullMessage = $"{message ?? ""}\nException: {exception?.Message}\nStackTrace: {exception?.StackTrace}";
            WriteLogAsync("FATAL", source, fullMessage);
        }

        private async void WriteLogAsync(string level, string source, string message)
        {
            try
            {
                var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {source}: {message}{Environment.NewLine}";
                Debug.WriteLine(logMessage);

                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(GetLogFileName(), CreationCollisionOption.OpenIfExists);
                await FileIO.AppendTextAsync(file, logMessage);
            }
            catch (Exception ex)
            {
                var fallback = $"[LoggerService] Error during save log: {ex.Message}\n{ex.StackTrace}";
                Debug.WriteLine(fallback);
            }
        }

        private static string GetLogFileName()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            return $"{LogFileName}_{date}.log";
        }
    }
}
