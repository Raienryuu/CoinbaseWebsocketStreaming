using System.Net.WebSockets;
using System.Text;

namespace StreamingWithBackpressure.Connections
{
    public class Connection
    {
        public string requestJson;
        public ClientWebSocket streamSocket;
        public Uri coinbaseFeed;
        public ArraySegment<byte> segment;
        public ArraySegment<byte> dataJson;

        public Connection()
        {
            requestJson = "";
            coinbaseFeed = new Uri("wss://ws-feed-public.sandbox.exchange.coinbase.com");
            streamSocket = new ClientWebSocket();

            byte[] buffer = new byte[10000];
            segment = new(buffer);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(requestJson);
            dataJson = new ArraySegment<byte>(jsonBytes);
        }

        async public Task TryStartConnectionAsync()
        {
            
            try
            {
                await streamSocket.ConnectAsync(coinbaseFeed, CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            await SendRequest();

            await ReceiveRequestConfirmation();
        }

        public void TryReconnecting()
        {
            if (streamSocket.State == WebSocketState.Aborted)
            {
                streamSocket = new ClientWebSocket();
            }          

            if (streamSocket.State == WebSocketState.Open)
            {
                Console.WriteLine("Connection has been restored.");
            }
            else if (streamSocket.State == WebSocketState.Aborted)
            {
                Console.WriteLine("Unable to restore WebSocket connection.");
            }
            else
            {
                throw new WebSocketException();
            }

        }

        async public Task ReceiveRequestConfirmation()
        {
            await streamSocket.ReceiveAsync(segment,
                CancellationToken.None);

            Console.WriteLine(Encoding.UTF8.GetString(segment));
            Console.WriteLine("--------");
        }

        async public Task SendRequest()
        {
            await streamSocket.SendAsync(dataJson, WebSocketMessageType.Text,
                true, CancellationToken.None);
        }

    }
    
}
