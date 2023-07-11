using System;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace StreamingWithBackpressure.Connections
{
    internal class SocketConnection
    {
        private Socket socket;
        private SslStream sslStream;
        private string serverHost = "ws-feed-public.sandbox.exchange.coinbase.com";
        private int serverPort = 443;

        public SocketConnection()
        {
            serverHost = "127.0.0.1"; serverPort = 8080;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(serverHost, serverPort);
            
            sslStream = new SslStream(new NetworkStream(socket));
        }

        public async Task StartConnectionSslStream()
        {
            sslStream.AuthenticateAsClient(serverHost);
            Debug.Assert(sslStream.IsAuthenticated);


            await UpgradeToSocketSslStream();

            string message = @"
            {
              ""type"": ""subscribe"",
              ""channels"": [{ ""name"": ""status""}]
            }";
            await SendThroughSslStream(message);
            await ReceiveThroughSslStream();
        }

        private async Task UpgradeToSocketSslStream()
        {
            string message = $"GET / HTTP/1.1\r\n" +
                 $"Host: {serverHost}\r\n" +
                 $"Upgrade: websocket\r\n" +
                 $"Connection: Upgrade\r\n" +
                 $"Sec-WebSocket-Key: AQIDBAUGBwgJCgsMDQ4PEC==\r\n" +
                 $"Sec-WebSocket-Version: 13\r\n\r\n";

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            sslStream.Write(messageBytes);
            await sslStream.FlushAsync();
            byte[] buffer = new byte[4000];
            int bytesRead = await sslStream.ReadAsync(buffer, CancellationToken.None);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received ssl: " + receivedMessage);
            return;
        }

        public async Task StartConnectionSocket()
        {
            string message = $"GET / HTTP/1.1\r\n" +
                 $"Host: {serverHost}\r\n" +
                 $"Upgrade: websocket\r\n" +
                 $"Connection: Upgrade\r\n" +
                 $"Sec-WebSocket-Key: AQIDBAUGBwgJCgsMDQ4PEC==\r\n" +
                 $"Sec-WebSocket-Version: 13\r\n\r\n";
            await SendThroughSocket(message);
            await ReceiveThroughSocket();

            message = @"
            {
              ""type"": ""subscribe"",
              ""channels"": [{ ""name"": ""status""}]
            }";
            await SendThroughSocket(message);
            await ReceiveThroughSocket();
        }

        private async Task SendThroughSslStream(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            messageBytes = MaskMessage(messageBytes);
            sslStream.Write(messageBytes);
            await sslStream.FlushAsync();
        }

        private async Task<string> ReceiveThroughSslStream()
        {
            byte[] buffer = new byte[4000];
            int bytesRead = await sslStream.ReadAsync(buffer, CancellationToken.None);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received ssl: " + receivedMessage);
            return receivedMessage;
        }

        private async Task SendThroughSocket(string message)
        {
            if(socket.Connected)
            {
                byte[] messageSegment = Encoding.UTF8
                                .GetBytes(message);
                await socket.SendAsync(messageSegment);
            }
            
        }

        private async Task<string> ReceiveThroughSocket()
        {
            byte[] responseBytes = new byte[500];
            await socket.ReceiveAsync(responseBytes);
            var responseMessage = Encoding.UTF8.GetString(responseBytes);
            Console.WriteLine("Received sock: " + responseMessage);
            return responseMessage;
        }

        private byte[] MaskMessage(byte[] message)
        {
            Random random = new Random();
            byte[] mask = new byte[4];
            random.NextBytes(mask);
            byte[] maskedMessage = new byte[message.Length + 6];
            maskedMessage[1] |= 0x80;
            Array.Copy(mask, 0, maskedMessage, 2, 4);
            for (int i = 0; i < message.Length; i++)
            {
                maskedMessage[i + 6] = (byte)(message[i] ^ mask[i % 4]);
            }
            return maskedMessage;
        }

    }
}
