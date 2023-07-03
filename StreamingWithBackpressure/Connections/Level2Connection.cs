using Newtonsoft.Json;
using StreamingWithBackpressure.ResponseModels;
using System.Linq;
using System.Net.WebSockets;
using System.Text;

namespace StreamingWithBackpressure.Connections
{
    public class Level2Connection : Connection, IDataRequester { 
        public Level2Connection()
        {
            requestJson = @"{
                ""type"": ""subscribe"",
                ""product_ids"": [
                    ""ETH-USD"",
                    ""BTC-USD""
                ],
                ""channels"": [""level2_batch""]
            }";

        }

        public async Task TryGetDataFromWebSocketAsync()
        {
            WebSocketReceiveResult res;
            string responseJson;
            var watch = new System.Diagnostics.Stopwatch();

            while (true)
            {
                try
                {
                    res = await this.streamSocket.ReceiveAsync(segment,
                    CancellationToken.None);

                    watch.Stop();

                    Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

                    byte[] receivedData = segment.Array!.Take(res.Count)
                                                       .ToArray();

                    responseJson = Encoding.UTF8.GetString(receivedData);
                    Console.WriteLine(responseJson);

                    if (responseJson is not null)
                    {
                        //var model = JsonConvert.DeserializeObject<StatusModel>(responseJson);
                        //Console.WriteLine(responseJson);

                        watch.Start();
                    }
                }
                catch (WebSocketException e)
                {
                    Console.WriteLine(e.Message);

                    TryReconnecting();
                }
            }
        }
    }
    
}
