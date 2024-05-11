namespace MyApp
{
    public class Metric
    {
        public int Id { get; private set; }
        public long Memory { get; private set; }
        public double ProcTime { get; private set; }
        public long FreeMemory { get; private set; }


        public Metric(long memory, double proctime, long freememory, int id = 0)
        {
            this.Id = id;
            this.Memory = memory;
            this.ProcTime = proctime;
            this.FreeMemory = freememory;
        }
    }
}