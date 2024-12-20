namespace CryptoexchangeMarketDepth.Services.Models
{
    public class Snapshot
    {
        public int Id { get; set; }
        public DateTime AcquiredAt { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
