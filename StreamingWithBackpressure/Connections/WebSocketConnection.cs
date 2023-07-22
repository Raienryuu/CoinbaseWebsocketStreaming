using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace StreamingWithBackpressure.Connections
{
    /// <summary>
    /// Instantiate only through <see cref="WebSocketConnectionFactory"/>.
    /// </summary>
    public class WebSocketConnection<T>
    {
        public string requestJson;
        public ClientWebSocket streamSocket;
        public Uri coinbaseFeed;
        public ArraySegment<byte> segment;
        public ArraySegment<byte> dataJson;

        public WebSocketConnection()
        {
            requestJson = "";
            coinbaseFeed = new Uri("wss://ws-feed-public.sandbox.exchange.coinbase.com");
            coinbaseFeed = new Uri("ws://127.0.0.1:8080/");
            streamSocket = new ClientWebSocket();
            byte[] jsonBytes = Encoding.UTF8.GetBytes(requestJson);
            dataJson = new ArraySegment<byte>(jsonBytes);
        }

        async public Task TryStartConnectionAsync()
        {

            try
            {
                await streamSocket.ConnectAsync(coinbaseFeed, CancellationToken.None);
            }
            catch (WebSocketException e)
            {
                Console.WriteLine(e.Message);
            }

            Debug.Assert(segment.Array is not null);

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
            else if (streamSocket.State == WebSocketState.Aborted 
                || streamSocket.State == WebSocketState.None)
            {
                Console.WriteLine("Unable to restore WebSocket connection.");
            }
            else
            {
                throw new WebSocketException(streamSocket.State.ToString());
            }
        }
        public void SetBufferSize(int bytes)
        {
            byte[] buffer = new byte[bytes];
            segment = new(buffer);
        }

        async private Task ReceiveRequestConfirmation()
        {
            try
            {
                await streamSocket.ReceiveAsync(segment,
                    CancellationToken.None);
            }
            catch (WebSocketException)
            {
                TryReconnecting();
            }
            Console.WriteLine(Encoding.UTF8.GetString(segment));
            Console.WriteLine("--------");
        }

        async private Task SendRequest()
        {
            dataJson = Encoding.UTF8.GetBytes(requestJson);
            await streamSocket.SendAsync(dataJson, WebSocketMessageType.Text,
                true, CancellationToken.None);
        }

        public async Task<T> GetDataFromWebSocketAsync()
        {
            if (streamSocket.State != WebSocketState.Open)
            {
                return default!;
            }
            WebSocketReceiveResult res;
            string responseJson;
            T model;

            try
            {
                res = await this.streamSocket.ReceiveAsync(segment,
                CancellationToken.None);
                Debug.Assert(segment.Array is not null);
                Debug.Assert(res.Count > 0);

                byte[] receivedData = segment.Array.Take(res.Count)
                                                    .ToArray();
                responseJson = Encoding.UTF8.GetString(receivedData);
                Console.WriteLine(responseJson);
                if (responseJson is null)
                {
                    throw new InvalidDataException();
                }

                model = JsonConvert.DeserializeObject<T>(responseJson)!;
                Debug.Assert(model is not null);
                return model;

            }
            catch (WebSocketException)
            {
                TryReconnecting();
            }
            return default!;
        }
        public void ClearBuffer()
        {
            byte[] buffer = new byte[10000];
            segment = new(buffer);
        }
    }

}
