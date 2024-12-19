using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CryptoexchangeMarketDepth.Clients.Integrations
{
    public class BitstampApiClient: IBitstampApiClient
    {

        private readonly HttpClient _httpClient;

        public BitstampApiClient(HttpClient httpClient, IOptions<BitstampApiOptions> options)
        {
            if (string.IsNullOrEmpty(options.Value.BaseUrl))
                throw new ArgumentException("Base URL for Bitstamp API is not configured.");

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        }

        /// <summary>
        /// Fetches the order book for a given market symbol.
        /// </summary>
        /// <param name="marketSymbol">The market symbol, e.g., "btceur".</param>
        /// <returns>OrderBookResponse object.</returns>
        public async Task<OrderBookResponse?> GetOrderBookAsync(string marketSymbol)
        {
            // Construct URL with query parameters
            var url = $"/api/v2/order_book/{marketSymbol}/?group=0";

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try
                {
                    return JsonSerializer.Deserialize<OrderBookResponse>(responseContent, options);
                }
                catch
                {
                    var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                    throw new Exception($"API Error: {error?.Reason}");
                }
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }
        }
    }

    // OrderBook Response Model
    public class OrderBookResponse
    {
        public List<List<string>> Asks { get; set; } = new();
        public List<List<string>> Bids { get; set; } = new();
        public string Microtimestamp { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
    }

    // Error Response Model
    public class ErrorResponse
    {
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
