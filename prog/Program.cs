namespace MyApp {
class MainClass
{
      static async Task Main(string[] args)
      {
            await Run();
      }

      static async Task Run()
      {
            // URL сервера Prometheus
            string prometheusUrl = "http://localhost:9090";

            // Создаем класс для получения метрик
            MetricsLoader metrics = new MetricsLoader(prometheusUrl);

            // Получение метрик:
            var fullmemory_query = await metrics.GetNodeMemoryMemTotalBytes();
            var fullmemory = MetricsLoader.GetValue(fullmemory_query);

            var proctime_query = await metrics.GetProcessCpuSecondsTotal();
            var proctime = MetricsLoader.GetValue(proctime_query);

            var freememory_query = await metrics.GetNodeFileSystemFreeBytes();
            var freememory = MetricsLoader.GetValue(freememory_query);

            // Вывод метрик в форматированном виде
            Console.WriteLine("Метрики Системы:");
            Console.WriteLine($"- Объем полной памяти (резидентный): {fullmemory}");
            Console.WriteLine($"- Общее время процессора (в секундах): {proctime}");
            Console.WriteLine($"- Свободное место в файловой системе: {freememory}");

            // Работа с БД
            var db = new Storage("leonid", "prometheus", "159372684gG", 5432);
            Metric metric = new Metric((long)Convert.ToDouble(fullmemory), Convert.ToDouble(proctime), (long)Convert.ToDouble(freememory));
            await db.InsertMetricAsync(metric);
            List<Metric> metricList = new List<Metric>(await db.GetAllMetricsAsync());
            foreach(var m in metricList)
            {
                  Console.WriteLine($"- Объем полной памяти (резидентный): {m.Memory}");
                  Console.WriteLine($"- Общее время процессора (в секундах): {m.ProcTime}");
                  Console.WriteLine($"- Свободное место в файловой системе: {m.FreeMemory}");
            }
      }
}
}