using CustomWallpaper.CrossCutting.Services;
using CustomWallpaper.Domain.Repositories;
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
        private readonly ILoggerService _logger;

        public WallpaperHistoryRepository(ILoggerService logger)
        {
            _logger = logger;
        }

        public async Task AddAsync(WallpaperHistory history)
        {
            try
            {
                using (var connection = DatabaseBootstrapper.GetConnection())
                {
                    await connection.OpenAsync();

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO WallpaperHistory (Id, ImageId, AppliedAt, Source)
                            VALUES (@Id, @ImageId, @AppliedAt, @Source)";

                        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                        cmd.Parameters.AddWithValue("@ImageId", history.ImageId);
                        cmd.Parameters.AddWithValue("@AppliedAt", history.AppliedAt);
                        cmd.Parameters.AddWithValue("@Source", history.Source);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                _logger.Info(nameof(WallpaperHistoryRepository), "WallpaperHistory registrado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryRepository), ex, $"Erro ao registrar histórico: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<WallpaperHistory>> GetAllAsync()
        {
            var list = new List<WallpaperHistory>();

            try
            {
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

                _logger.Info(nameof(WallpaperHistoryRepository), "Histórico completo de wallpapers carregado.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryRepository), ex, $"Erro ao carregar históricos: {ex.Message}");
                throw;
            }

            return list;
        }

        public async Task<IEnumerable<WallpaperHistoryDto>> GetAllWithImageNameAsync()
        {
            var list = new List<WallpaperHistoryDto>();

            try
            {
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
                                    Id = reader.GetString(0),
                                    ImageId = reader.GetString(1),
                                    FileName = reader.GetString(2),
                                    AppliedAt = reader.GetString(3),
                                    Source = reader.GetString(4)
                                });
                            }
                        }
                    }
                }

                _logger.Info(nameof(WallpaperHistoryRepository), "Histórico com nomes de imagem carregado.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryRepository), ex, $"Erro ao carregar históricos com nomes: {ex.Message}");
                throw;
            }

            return list;
        }

        public async Task<WallpaperHistory> GetLastAppliedAsync()
        {
            try
            {
                using (var connection = DatabaseBootstrapper.GetConnection())
                {
                    await connection.OpenAsync();

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM WallpaperHistory ORDER BY AppliedAt DESC LIMIT 1";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                _logger.Info(nameof(WallpaperHistoryRepository), "Último wallpaper aplicado carregado.");
                                return MapHistory(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(WallpaperHistoryRepository), ex, $"Erro ao obter último wallpaper aplicado: {ex.Message}");
                throw;
            }

            return null;
        }

        private WallpaperHistory MapHistory(SqliteDataReader reader)
        {
            return new WallpaperHistory
            {
                Id = reader.GetString(0),
                ImageId = reader.GetString(1),
                AppliedAt = reader.GetString(2),
                Source = reader.GetString(3)
            };
        }
    }
}
