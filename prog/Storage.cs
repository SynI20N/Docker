using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace MyApp
{
      public class Storage
      {
            private readonly string _connectionString;

            public Storage(string username, string database, string password, int port)
            {
                  _connectionString = $"Host=localhost;Username={username};Database={database};Port={port};Password={password}";
            }

            // Create: Вставка новой строки в таблицу metrics
            public async Task<int> InsertMetricAsync(Metric metric)
            {
                  var query = "INSERT INTO metrics (memory, proc_time, free_mem) VALUES (@memory, @proc_time, @free_mem) RETURNING id;";
                  using (var conn = new NpgsqlConnection(_connectionString))
                  {
                        await conn.OpenAsync();
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                              cmd.Parameters.AddWithValue("@memory", metric.Memory);
                              cmd.Parameters.AddWithValue("@proc_time", metric.ProcTime);
                              cmd.Parameters.AddWithValue("@free_mem", metric.FreeMemory);

                              return (int)await cmd.ExecuteScalarAsync(); // Возвращает ID новой строки
                        }
                  }
            }

            // Read: Чтение всех строк из таблицы metrics
            public async Task<List<Metric>> GetAllMetricsAsync()
            {
                  var query = "SELECT id, memory, proc_time, free_mem FROM metrics;";
                  var metrics = new List<Metric>();
                  using (var conn = new NpgsqlConnection(_connectionString))
                  {
                        await conn.OpenAsync();
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                              using (var reader = await cmd.ExecuteReaderAsync())
                              {
                                    while (await reader.ReadAsync())
                                    {
                                          var metric = new Metric(
                                                (long)Convert.ToInt64(reader["memory"]),
                                                (double)Convert.ToDouble(reader["proc_time"]),
                                                (long)Convert.ToInt64(reader["free_mem"]),
                                                (int)Convert.ToInt32(reader["id"])
                                          );
                                          metrics.Add(metric);
                                    }
                              }
                        }
                  }

                  return metrics;
            }

            // Update: Обновление существующей строки в таблице metrics
            public async Task<int> UpdateMetricAsync(int id, Metric metric)
            {
                  var query = "UPDATE metrics SET memory = @memory, proc_time = @proc_time, free_mem = @free_mem WHERE id = @id;";
                  using (var conn = new NpgsqlConnection(_connectionString))
                  {
                        await conn.OpenAsync();
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                              cmd.Parameters.AddWithValue("@id", id);
                              cmd.Parameters.AddWithValue("@memory", metric.Memory);
                              cmd.Parameters.AddWithValue("@proc_time", metric.ProcTime);
                              cmd.Parameters.AddWithValue("@free_mem", metric.FreeMemory);

                              return await cmd.ExecuteNonQueryAsync(); // Возвращает количество обновленных строк
                        }
                  }
            }

            // Delete: Удаление строки из таблицы metrics
            public async Task<int> DeleteMetricAsync(int id)
            {
                  var query = "DELETE FROM metrics WHERE id = @id;";
                  using (var conn = new NpgsqlConnection(_connectionString))
                  {
                        await conn.OpenAsync();
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                              cmd.Parameters.AddWithValue("@id", id);

                              return await cmd.ExecuteNonQueryAsync(); // Возвращает количество удаленных строк
                        }
                  }
            }
      }
}
