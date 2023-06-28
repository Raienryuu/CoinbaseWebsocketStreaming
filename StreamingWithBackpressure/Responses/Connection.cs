using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;

namespace StreamingWithBackpressure.Responses
{
    public class Connection
    {
        public string requestJson;
        public ClientWebSocket streamSocket;
        public Uri coinbaseFeed;
        private readonly DatabaseContext dbContext;

        public Connection(DatabaseContext dbContext)
        {
            requestJson = "";
            coinbaseFeed = new Uri("wss://ws-feed-public.sandbox.exchange.coinbase.com");
            streamSocket = new ClientWebSocket();
            this.dbContext = dbContext;
        }


        public virtual async void StartStatusRequest()
        {
        }

        async public Task StartConnection()
        {
            Console.WriteLine(dbContext.Database.CanConnect());
            try
            {
                await streamSocket.ConnectAsync(coinbaseFeed, CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            byte[] jsonBytes = Encoding.UTF8.GetBytes(requestJson);
            ArraySegment<byte> data = new ArraySegment<byte>(jsonBytes);

            await streamSocket.SendAsync(data, WebSocketMessageType.Text,
                true, CancellationToken.None);
            

            byte[] buffer = new byte[10000];
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            WebSocketReceiveResult res = await streamSocket.ReceiveAsync(segment,
                CancellationToken.None);

            Console.WriteLine(Encoding.UTF8.GetString(segment));
            Console.WriteLine("--------");
            
        }
        
    }
}
