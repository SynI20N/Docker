using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace MyApp
{
    class MetricsLoader
    {
        private readonly string _prometheusUrl;
        private HttpClient _httpClient;

        public MetricsLoader(string prometheusUrl)
        {
            _prometheusUrl = prometheusUrl;
            _httpClient = new HttpClient();
        }

        // Общее количество физической памяти на узле
        public async Task<JsonObject> GetNodeMemoryMemTotalBytes()
        {
            return await QueryMetric("node_memory_MemTotal_bytes");
        }

        // Общее количество времени процессора, затраченного на выполнение процесса
        public async Task<JsonObject> GetProcessCpuSecondsTotal()
        {
            return await QueryMetric("process_cpu_seconds_total");
        }

        // Свободное место на файловой системе
        public async Task<JsonObject> GetNodeFileSystemFreeBytes()
        {
            return await QueryMetric("node_filesystem_free_bytes");
        }

        // Общее количество принятых байтов через сетевой интерфейс
        public async Task<JsonObject> GetNodeNetworkReceiveBytesTotal()
        {
            return await QueryMetric("node_network_receive_bytes_total");
        }

        //Получение значения из HTTP-запроса
        public static string GetValue(JsonObject json)
        {
            try
            {
                var path = new[] { "data", "result", "index-0", "value", "index-1" };

                JsonNode current = json;

                foreach (var key in path)
                {
                    string[] parts = key.Split('-');
                    if(parts.Length == 2)
                    {
                        if (Int32.TryParse(parts[1], out int result))
                        {
                            current = current[result];
                        }
                    }
                    else 
                    {
                        current = current[key];
                    }
                }

                return current?.ToString() ?? "null";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при извлечении значения: {ex.Message}");
                return null;
            }
        }

        private async Task<JsonObject> QueryMetric(string metric)
        {
            // Формируем URL для запроса метрики
            string queryUrl = $"{_prometheusUrl}/api/v1/query?query={metric}";

            try
            {
                // Отправляем GET-запрос к серверу Prometheus
                HttpResponseMessage response = await _httpClient.GetAsync(queryUrl);

                // Проверяем успешность запроса
                if (response.IsSuccessStatusCode)
                {
                    // Получаем ответ в виде строки
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Преобразование string в json
                    JsonObject jsonResponse = (JsonObject)JsonNode.Parse(responseContent);

                    return jsonResponse;
                }
                else
                {
                    JsonObject jsonObj = new JsonObject { ["error"] = response.StatusCode.ToString() };
                    return jsonObj;
                }
            }
            catch (Exception ex)
            {
                JsonObject jsonObj = new JsonObject { ["error"] = ex.ToString() };
                return jsonObj;
            }
        }
    }
}
