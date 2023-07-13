using StreamingWithBackpressure.Connections.DataModels;
using StreamingWithBackpressure.NewFolder;
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
        private string serverHost;
        private int serverPort;

        public SocketConnection(string serverHost, int serverPort)
        {
            this.serverHost = serverHost;
            this.serverPort = serverPort;
            //serverHost = "127.0.0.1"; serverPort = 8080;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(serverHost, serverPort);
            sslStream = new SslStream(new NetworkStream(socket));
        }

        public async Task StartConnectionSslStream()
        {
            sslStream.AuthenticateAsClient(serverHost);
            Debug.Assert(sslStream.IsAuthenticated);
            await UpgradeToSocketSslStream();
            string message = RequestExample.StatusChannelRequest;
            await SendThroughSslStream(message);
            await ReceiveThroughSslStream();
        }

        private async Task UpgradeToSocketSslStream()
        {
            string message = RequestExample.UpgradeToWebsocket(serverHost);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            sslStream.Write(messageBytes);
            await sslStream.FlushAsync();
            await ReceiveThroughSslStream();
        }

        private async Task UpgradeToSocket()
        {
            string message = RequestExample.UpgradeToWebsocket(serverHost);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(messageBytes);
            await ReceiveThroughSocket(); 
        }

        public async Task StartConnectionSocket()
        {

            await UpgradeToSocket();

            var message = RequestExample.StatusChannelRequest;
            await SendThroughSocket(message);
            await ReceiveThroughSocket();
        }

        private async Task SendThroughSslStream(string message)
        {
            SocketPayload payload = new();
            payload.SetFinBit(true);
            payload.SetOpcode(Opcode.TextFrame);
            payload.SetApplicationData(message);
            byte[] messageBytes = payload.GetMessageBytes();

            await sslStream.WriteAsync(messageBytes, 0, messageBytes.Length);
            await sslStream.FlushAsync();
        }

        private async Task<string> ReceiveThroughSslStream()
        {
            byte[] buffer = new byte[4000];
            int bytesRead = await sslStream.ReadAsync(buffer, CancellationToken.None);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received SslStream: " + receivedMessage);
            return receivedMessage;
        }

        private async Task SendThroughSocket(string message)
        {
            if(socket.Connected)
            {
                SocketPayload payload = new SocketPayload();
                payload.SetFinBit(true);
                payload.SetOpcode(Opcode.BinaryFrame);
                payload.SetApplicationData(message);
                byte[] messageBytes = payload.GetMessageBytes();

                await socket.SendAsync(messageBytes);
            }
        }

        private async Task<string> ReceiveThroughSocket()
        {
            byte[] responseBytes = new byte[500];
            await socket.ReceiveAsync(responseBytes);
            var responseMessage = Encoding.UTF8.GetString(responseBytes);
            Console.WriteLine("Received socket: " + responseMessage);
            return responseMessage;
        }
    }
}
