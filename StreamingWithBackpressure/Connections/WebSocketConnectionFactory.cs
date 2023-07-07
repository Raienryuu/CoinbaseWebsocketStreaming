using StreamingWithBackpressure.Connections.DataModels;

namespace StreamingWithBackpressure.Connections
{
    /// <summary>
    /// Allows creating predefined instances of <see cref="WebSocketConnection{T}"/>.
    /// </summary>
    public class WebSocketConnectionFactory
    {
        public WebSocketConnection<Level2Model> GetLevel2Connection()
        {
            WebSocketConnection<Level2Model> connection = new();

            string requestJson = @"
            {
               ""type"":""subscribe"",
               ""product_ids"":[ ""BTC-USD""],
               ""channels"":[""level2_batch""]
            }";

            connection.requestJson = requestJson;
            connection.SetBufferSize(400000);

            return connection;
        }

        public WebSocketConnection<StatusModel> GetStatusConnection()
        {
            WebSocketConnection<StatusModel> connection = new();

            string requestJson = @"
            {
              ""type"": ""subscribe"",
              ""channels"": [{ ""name"": ""status""}]
            }";

            connection.requestJson = requestJson;
            connection.SetBufferSize(10000);

            return connection;
        }
    }
}
