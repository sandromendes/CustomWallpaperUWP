using System;

namespace CustomWallpaper.Core.Services
{
    public interface ILoggerService
    {
        void Info(string source, string message);
        void Error(string source, Exception exception, string message = null);
        void Fatal(string source, Exception exception, string message = null);
    }
}
