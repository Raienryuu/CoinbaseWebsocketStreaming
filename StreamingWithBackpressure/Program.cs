using StreamingWithBackpressure.BackpressureSimulation;
using StreamingWithBackpressure.Connections;
using StreamingWithBackpressure.Connections.DataModels;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;

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
            WebSocketConnectionFactory connectionBuilder = new();

            FastFakeIngester fastFakeIngester = new();
            MediumFakeIngester mediumFakeIngester = new();
            SlowFakeIngester slowFakeIngester = new();

            WebSocketConnection<Level2Model> statusConnection = connectionBuilder.GetLevel2Connection();
            //await statusConnection.TryStartConnectionAsync();

            //BackpressureResolvingMethod method;
            //method = BackpressureResolvingMethod.BUFFER;

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

            //statusConnection.TryStartConnectionAsync();

            SocketConnection socketConnection = new();
            await socketConnection.StartConnectionSocket();

            Console.ReadKey();
        }

    }
}