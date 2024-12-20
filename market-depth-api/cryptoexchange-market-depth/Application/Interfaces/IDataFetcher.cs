namespace CryptoexchangeMarketDepth.Application.Interfaces
{
    public interface IDataFetcher
    {
        Task FetchAndStoreDataAsync();
    }
}