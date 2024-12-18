namespace CryptoexchangeMarketDepth.Models
{
    public class OrderBookSnapshot
    {
        public int Id { get; set; }
        public DateTime AcquiredAt { get; set; } 
        public DateTime Timestamp { get; set; }
        public DateTime Microtimestamp { get; set; }

        public string MarketSymbol { get; set; }

        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public ICollection<Ask> Asks { get; set; } = new List<Ask>();
    }
}
