namespace CryptoexchangeMarketDepth.Services.Interfaces
{
    public interface IMarketDepthService
    {
        Task<ComputedMarketDepthResult> ComputeMarketDepthAsync();
    }
}