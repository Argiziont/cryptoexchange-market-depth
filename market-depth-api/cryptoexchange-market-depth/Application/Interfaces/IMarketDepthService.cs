using CryptoexchangeMarketDepth.Application.DTOs;

namespace CryptoexchangeMarketDepth.Application.Interfaces
{
    public interface IMarketDepthService
    {
        Task<ComputedMarketDepthResult> ComputeMarketDepthAsync();
    }
}