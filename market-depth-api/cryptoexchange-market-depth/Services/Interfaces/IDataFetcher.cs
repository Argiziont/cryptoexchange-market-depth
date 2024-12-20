namespace CryptoexchangeMarketDepth.Services.Interfaces
{
    public interface IDataFetcher
    {
        Task FetchAndStoreDataAsync();
    }
}