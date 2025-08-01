using CustomWallpaper.Domain.Repositories;
using CustomWallpaper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Infrastructure.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        public async Task AddAsync(Folder folder)
        {
            if (await ExistsAsync(folder.FolderPath))
                return;

            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();

                command.CommandText = @"
                    INSERT INTO Folders (Id, FolderPath, FolderName, DateAdded, AccessToken)
                    VALUES ($id, $path, $name, $date, $token)";

                command.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("$path", folder.FolderPath);
                command.Parameters.AddWithValue("$name", folder.FolderName);
                command.Parameters.AddWithValue("$date", folder.DateAdded.ToString("o"));
                command.Parameters.AddWithValue("$token", folder.AccessToken);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<Folder>> GetAllAsync()
        {
            var folders = new List<Folder>();

            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Folders";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        folders.Add(new Folder
                        {
                            Id = reader.GetString(0),
                            FolderPath = reader.GetString(1),
                            FolderName = reader.GetString(2),
                            DateAdded = DateTime.Parse(reader.GetString(3)),
                            AccessToken = reader.GetString(4),
                        });
                    }
                }
            }

            return folders;
        }

        public async Task<bool> ExistsAsync(string folderPath)
        {
            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT 1 FROM Folders WHERE FolderPath = $path LIMIT 1";
                command.Parameters.AddWithValue("$path", folderPath);

                var result = await command.ExecuteScalarAsync();
                return result != null;
            }
        }

        public async Task RemoveAsync(int id)
        {
            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();

                command.CommandText = "DELETE FROM Folders WHERE Id = $id";
                command.Parameters.AddWithValue("$id", id);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
