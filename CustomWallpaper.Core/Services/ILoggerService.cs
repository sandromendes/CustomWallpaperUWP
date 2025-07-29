using System;

namespace CustomWallpaper.CrossCutting.Services
{
    public interface ILoggerService
    {
        string LogFileName { get; set; }

        void Info(string source, string message);
        void Error(string source, Exception exception, string message = null);
        void Fatal(string source, Exception exception, string message = null);
    }
}
