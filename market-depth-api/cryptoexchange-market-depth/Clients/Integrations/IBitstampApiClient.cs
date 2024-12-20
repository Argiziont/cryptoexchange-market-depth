namespace CryptoexchangeMarketDepth.Clients.Integrations
{
    public interface IBitstampApiClient
    {
        Task<OrderBookResponse?> GetOrderBookAsync(string marketSymbol);
    }

}
