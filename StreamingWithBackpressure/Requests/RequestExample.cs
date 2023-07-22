namespace StreamingWithBackpressure.Requests
{
    internal static class RequestExample
    {
        public static string UpgradeToWebsocket(string serverHost)
        {
            string request = $"GET / HTTP/1.1\r\n" +
                 $"Host: {serverHost}\r\n" +
                 $"Upgrade: websocket\r\n" +
                 $"Connection: Upgrade\r\n" +
                 $"Sec-WebSocket-Key: AQIDBAUGBwgJCgsMDQ4PEC==\r\n" +
                 $"Sec-WebSocket-Version: 13\r\n\r\n";
            return request;
        }

        public static string StatusChannelRequest = @"
            {
              ""type"": ""subscribe"",
              ""channels"": [{ ""name"": ""status""}]
            }";

        public static string Level2ChannelRequest =@"
            {
               ""type"":""subscribe"",
               ""product_ids"":[ ""BTC-USD""],
               ""channels"":[""level2_batch""]
            }";
    }
}
