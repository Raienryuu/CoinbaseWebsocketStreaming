namespace StreamingWithBackpressure.Connections
{
    public class ConnectionBuilder
    {
        public Connection GetLevel2Connection(string jsonRequest)
        {
            jsonRequest = @"{
                ""type"": ""subscribe"",
                ""product_ids"": [
                    ""ETH-USD"",
                    ""BTC-USD""
                ],
                ""channels"": [""level2_batch""]
            }";
            return new Connection(jsonRequest);
        }
    }
}
