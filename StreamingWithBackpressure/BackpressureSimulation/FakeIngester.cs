namespace StreamingWithBackpressure.BackpressureSimulation
{
    public class FastFakeIngester : IDataIngester
    {
        public async Task IngestData()
        {
            Thread.Sleep(1000);
            Console.WriteLine("execution took: 1000ms");
        }
    }
    public class MediumFakeIngester : IDataIngester
    {
        public async Task IngestData()
        {
            Thread.Sleep(5000);
            Console.WriteLine("execution took: 5000ms");

        }
    }
    public class SlowFakeIngester : IDataIngester
    {
        public async Task IngestData()
        {
            Thread.Sleep(30000);
            Console.WriteLine("execution took: 30000ms");

        }
    }
}
