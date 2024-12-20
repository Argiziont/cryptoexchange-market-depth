namespace CryptoexchangeMarketDepth.Application.DTOs
{
    public class ComputedMarketDepthResult
    {
        public List<DepthChartPoint> Data { get; set; } = new List<DepthChartPoint>();
    }
}