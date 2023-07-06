using StreamingWithBackpressure.Connections.DataModels;

namespace StreamingWithBackpressure.Connections
{
    /// <summary>
    /// Allows creating predefined instances of <see cref="Connection{T}"/>.
    /// </summary>
    public class ConnectionFactory
    {
        public Connection<Level2Model> GetLevel2Connection()
        {
            Connection<Level2Model> connection = new();

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

        public Connection<StatusModel> GetStatusConnection()
        {
            Connection<StatusModel> connection = new();

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
