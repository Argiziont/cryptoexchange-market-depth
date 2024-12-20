namespace CryptoexchangeMarketDepth.Infrastructure.ExternalServices.Integrations
{
    public interface IBitstampApiClient
    {
        Task<OrderBookResponse?> GetOrderBookAsync(string marketSymbol);
    }

}
