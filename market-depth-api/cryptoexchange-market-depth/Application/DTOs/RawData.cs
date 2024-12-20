namespace CryptoexchangeMarketDepth.Application.DTOs
{
    public class RawData
    {
        public DateTime AcquiredAt { get; set; }
        public DateTime Timestamp { get; set; }

        public IEnumerable<RawApiOrder> Bids { get; set; } = new List<RawApiOrder>();
        public IEnumerable<RawApiOrder> Asks { get; set; } = new List<RawApiOrder>();
    }

    public class RawApiOrder
    {
        public string Price { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;

    }
}
