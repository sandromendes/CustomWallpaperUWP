using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace CustomWallpaper.Infrastructure
{
    public static class DatabaseBootstrapper
    {
        private static readonly string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "wallpaper.db");

        public static async Task InitializeAsync()
        {
            if (!File.Exists(DbPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DbPath));
            }

            using (var connection = new SqliteConnection($"Data Source={DbPath}"))
            {
                await connection.OpenAsync();

                var createImagesTable = @"
                    CREATE TABLE IF NOT EXISTS Images (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FileName TEXT NOT NULL,
                        FilePath TEXT NOT NULL,
                        FileExtension TEXT,
                        FileSizeInBytes INTEGER,
                        DateCreated TEXT,
                        DateModified TEXT,
                        Width INTEGER,
                        Height INTEGER,
                        Hash TEXT UNIQUE,
                        IsFavorite INTEGER
                    );
                ";

                var createWallpaperHistory = @"
                    CREATE TABLE IF NOT EXISTS WallpaperHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ImageId INTEGER NOT NULL,
                        AppliedAt TEXT NOT NULL,
                        Source TEXT,
                        FOREIGN KEY(ImageId) REFERENCES Images(Id)
                    );
                ";

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = createImagesTable;
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText = createWallpaperHistory;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={DbPath}");
        }
    }
}
