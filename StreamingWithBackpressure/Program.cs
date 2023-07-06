using StreamingWithBackpressure.BackpressureSimulation;
using StreamingWithBackpressure.Connections;
using StreamingWithBackpressure.Connections.DataModels;

namespace StreamingWithBackpressure
{
    public class Program
    {
        private enum BackpressureResolvingMethod
        {
            BUFFER = 0,
            SKIP = 1,
            CONTROL_SENDER = 2
        }
        public static async Task Main(string[] args)
        {
            ConnectionFactory connectionBuilder = new();

            FastFakeIngester fastFakeIngester = new();
            MediumFakeIngester mediumFakeIngester = new();
            SlowFakeIngester slowFakeIngester = new();

            Connection<Level2Model> statusConnection = connectionBuilder.GetLevel2Connection();
            await statusConnection.TryStartConnectionAsync();

            BackpressureResolvingMethod method;
            method = BackpressureResolvingMethod.BUFFER;

            //switch (method)
            //{
            //    case BackpressureResolvingMethod.BUFFER:
            //        while (true)
            //        {
            //            await statusConnection.GetDataFromWebSocketAsync();
            //            await slowFakeIngester.IngestData();
            //        }
            //    case BackpressureResolvingMethod.SKIP:
            //        while (true) { 
            //            await statusConnection.GetDataFromWebSocketAsync();
            //            await mediumFakeIngester.IngestData();
            //        }
            //    case BackpressureResolvingMethod.CONTROL_SENDER:
            //        while (true)
            //        {
            //            await statusConnection.GetDataFromWebSocketAsync();
            //            await mediumFakeIngester.IngestData();
            //        }
            //    default:
            //        throw new Exception("Unindentified resolving method.");

            //}


            Console.ReadKey();
        }

    }
}