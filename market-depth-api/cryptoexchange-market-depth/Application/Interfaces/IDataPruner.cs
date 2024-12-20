namespace CryptoexchangeMarketDepth.Application.Interfaces
{
    public interface IDataPruner
    {
        Task PruneOldDataAsync();
    }
}