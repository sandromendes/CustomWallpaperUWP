using CustomWallpaper.Domain.Application;
using CustomWallpaper.Domain.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        public async Task<IEnumerable<Image>> GetAllAsync()
        {
            var list = new List<Image>();

            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Images";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(MapImage(reader));
                        }
                    }
                }
            }
            return list;
        }

        public async Task<Image> GetByIdAsync(int id)
        {
            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Images WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync() ? MapImage(reader) : null;
                    }
                }
            }
        }

        public async Task<Image> GetByHashAsync(string hash)
        {
            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Images WHERE Hash = @hash";
                    cmd.Parameters.AddWithValue("@hash", hash);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync() ? MapImage(reader) : null;
                    }
                }
            }
        }

        public async Task AddAsync(Image image)
        {
            var existing = await GetByHashAsync(image.Hash);
            if (existing != null)
                return;

            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Images 
                    (FileName, FilePath, FileExtension, FileSizeInBytes, DateCreated, DateModified, Width, Height, Hash, IsFavorite)
                    VALUES (@FileName, @FilePath, @FileExtension, @FileSize, @Created, @Modified, @Width, @Height, @Hash, @Fav)";

                    cmd.Parameters.AddWithValue("@FileName", image.FileName);
                    cmd.Parameters.AddWithValue("@FilePath", image.FilePath);
                    cmd.Parameters.AddWithValue("@FileExtension", image.FileExtension);
                    cmd.Parameters.AddWithValue("@FileSize", image.FileSizeInBytes);
                    cmd.Parameters.AddWithValue("@Created", image.DateCreated);
                    cmd.Parameters.AddWithValue("@Modified", image.DateModified);
                    cmd.Parameters.AddWithValue("@Width", image.Width);
                    cmd.Parameters.AddWithValue("@Height", image.Height);
                    cmd.Parameters.AddWithValue("@Hash", image.Hash);
                    cmd.Parameters.AddWithValue("@Fav", image.IsFavorite ? 1 : 0);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> ExistsAsync(string hash)
        {
            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT 1 FROM Images WHERE Hash = @hash LIMIT 1";
                    cmd.Parameters.AddWithValue("@hash", hash);
                    var result = await cmd.ExecuteScalarAsync();
                    return result != null;
                }
            }
        }

        public async Task UpdateAsync(Image image)
        {
            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Images 
                    SET 
                        FileName = $FileName,
                        FilePath = $FilePath,
                        FileExtension = $FileExtension,
                        FileSizeInBytes = $FileSizeInBytes,
                        DateCreated = $DateCreated,
                        DateModified = $DateModified,
                        Width = $Width,
                        Height = $Height,
                        Hash = $Hash,
                        IsFavorite = $IsFavorite
                    WHERE Id = $Id";

                command.Parameters.AddWithValue("$Id", image.Id);
                command.Parameters.AddWithValue("$FileName", image.FileName);
                command.Parameters.AddWithValue("$FilePath", image.FilePath);
                command.Parameters.AddWithValue("$FileExtension", image.FileExtension);
                command.Parameters.AddWithValue("$FileSizeInBytes", image.FileSizeInBytes);
                command.Parameters.AddWithValue("$DateCreated", image.DateCreated);
                command.Parameters.AddWithValue("$DateModified", image.DateModified);
                command.Parameters.AddWithValue("$Width", image.Width);
                command.Parameters.AddWithValue("$Height", image.Height);
                command.Parameters.AddWithValue("$Hash", image.Hash);
                command.Parameters.AddWithValue("$IsFavorite", image.IsFavorite ? 1 : 0);

                await command.ExecuteNonQueryAsync();
            }
        }


        private Image MapImage(SqliteDataReader reader)
        {
            DateTime? modified;
            if (reader.IsDBNull(6))
                modified = null;
            else
                modified = DateTime.Parse(reader.GetString(6));

            return new Image
            {
                Id = reader.GetInt32(0),
                FileName = reader.GetString(1),
                FilePath = reader.GetString(2),
                FileExtension = reader.IsDBNull(3) ? null : reader.GetString(3),
                FileSizeInBytes = reader.GetInt64(4),
                DateCreated = reader.GetString(5),
                DateModified = modified.ToString(),
                Width = reader.GetInt32(7),
                Height = reader.GetInt32(8),
                Hash = reader.GetString(9),
                IsFavorite = reader.GetInt32(10) == 1
            };
        }
    }
}
