using CustomWallpaper.Domain.Application;
using CustomWallpaper.Domain.Entities;
using CustomWallpaper.Domain.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWallpaper.Infrastructure.Repositories
{
    public class WallpaperHistoryRepository : IWallpaperHistoryRepository
    {
        public async Task AddAsync(WallpaperHistory history)
        {
            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO WallpaperHistory (ImageId, AppliedAt, Source)
                    VALUES (@ImageId, @AppliedAt, @Source)";
                    cmd.Parameters.AddWithValue("@ImageId", history.ImageId);
                    cmd.Parameters.AddWithValue("@AppliedAt", history.AppliedAt);
                    cmd.Parameters.AddWithValue("@Source", history.Source);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<WallpaperHistory>> GetAllAsync()
        {
            var list = new List<WallpaperHistory>();

            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM WallpaperHistory";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(MapHistory(reader));
                        }
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<WallpaperHistoryDto>> GetAllWithImageNameAsync()
        {
            var list = new List<WallpaperHistoryDto>();

            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            h.Id,
                            h.ImageId,
                            i.FileName,
                            h.AppliedAt,
                            h.Source
                        FROM 
                            WallpaperHistory h
                        INNER JOIN 
                            Images i ON h.ImageId = i.Id
                        ORDER BY 
                            h.AppliedAt DESC";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new WallpaperHistoryDto
                            {
                                Id = reader.GetInt32(0),
                                ImageId = reader.GetInt32(1),
                                FileName = reader.GetString(2),
                                AppliedAt = reader.GetString(3),
                                Source = reader.GetString(4)
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<WallpaperHistory> GetLastAppliedAsync()
        {
            using (var connection = DatabaseBootstrapper.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM WallpaperHistory ORDER BY AppliedAt DESC LIMIT 1";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync() ? MapHistory(reader) : null;
                    }
                }
            }
        }

        private WallpaperHistory MapHistory(SqliteDataReader reader)
        {
            return new WallpaperHistory
            {
                Id = reader.GetInt32(0),
                ImageId = reader.GetInt32(1),
                AppliedAt = reader.GetString(2),
                Source = reader.GetString(3)
            };
        }
    }
}
