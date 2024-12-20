namespace CryptoexchangeMarketDepth.Services.Interfaces
{
    public interface IDataPruner
    {
        Task PruneOldDataAsync();
    }
}