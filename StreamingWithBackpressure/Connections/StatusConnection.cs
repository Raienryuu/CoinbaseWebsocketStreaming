using Newtonsoft.Json;
using StreamingWithBackpressure.Connections;
using System.Linq;
using System.Net.WebSockets;
using System.Text;

namespace StreamingWithBackpressure.ResponseModels
{
    public class Details
    {
        public string? type;
        public string? symbol;
        public int network_confirmations;
        public int sort_order;
        public string? crypto_address_link;
        public string? crypto_transaction_link;
        public required List<string> push_payment_methods;
        public required List<string> group_types;
        public string? display_name;
    }

    public class SupportedNetwork
    {
        public string? id;
        public string? name;
        public string? status;
        public string? contract_address;
        public string? crypto_address_link;
        public string? crypto_transaction_link;
        public double min_withdrawal_amount;
        public double max_withdrawal_amount;
        public int network_confirmations;
        public int processing_time_seconds;
    }
    public class StatusProductModel
    {
        public string? id;
        public string? base_currency;
        public string? quote_currency;
        public double base_increment;
        public double quote_increment;
        public string? display_name;
        public string? status;
        public bool margin_enabled;
        public string? status_message;
        public double min_market_funds;
        public bool post_only;
        public bool limit_only;
        public bool cancel_only;
        public bool auction_mode;
        public string? type;
        public bool fx_stablecoin;
        public double max_slippage_percentage;
    }

    public class StatusCurrencyModel
    {
        public string? id;
        public string? name;
        public double min_size;
        public string? status;
        public string? funding_account_id;
        public string? status_message;
        public double max_precision;
        public required List<string> convertible_to;
        public required Details details;
        public string? default_network;
        public List<SupportedNetwork>? supported_networks;
        public object ?network_map;
    }

    public class StatusModel
    {
        public required string type;
        public required List<StatusProductModel> products;
        public required List<StatusCurrencyModel> currencies;
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    public class StatusConnection : Connection, IDataRequester
    {
        public StatusConnection() : base()
        {
            requestJson = @"
            {
              ""type"": ""subscribe"",
              ""channels"": [{ ""name"": ""status""}]
            }
            ";

            byte[] buffer = new byte[10000];
            segment = new(buffer);
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
                    if (responseJson is not null)
                    {
                        var model = JsonConvert.DeserializeObject<StatusModel>(responseJson);
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
