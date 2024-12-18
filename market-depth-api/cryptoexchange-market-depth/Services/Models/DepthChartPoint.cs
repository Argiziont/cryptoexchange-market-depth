namespace CryptoexchangeMarketDepth.Services
{
    public class DepthChartPoint
    {
        public double Price { get; set; }
        public double? BidsDepth { get; set; }
        public double? AsksDepth { get; set; }
    }
}