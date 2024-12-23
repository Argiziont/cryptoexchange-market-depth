﻿namespace CryptoexchangeMarketDepth.Shared
{
    public class Bid
    {
        public int Id { get; set; }
        public int OrderBookSnapshotId { get; set; }
        public string Price { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;

        public OrderBookSnapshot? Snapshot { get; set; }
    }
}
