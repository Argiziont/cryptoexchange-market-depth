namespace CryptoexchangeMarketDepth.Application.Options
{
    public class FetcherServiceOptions
    {
        public string MarketSymbol { get; set; } = string.Empty;
        public int FetchDelay { get; set; } = 5;
        public int DataLiveTime { get; set; } = 900;
        public int PrunerDelay { get; set; } = 60;
    }
}
