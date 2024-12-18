﻿using CryptoexchangeMarketDepth.Context;
using CryptoexchangeMarketDepth.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoexchangeMarketDepth.Services
{
    public class MarketDepthComputer
    {
        private readonly OrderBookDbContext _dbContext;
        private readonly IDbContextFactory<OrderBookDbContext> _dbContextFactory;

        public MarketDepthComputer(OrderBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ComputedMarketDepthResult> ComputeMarketDepthAsync()
        {   
            var latestSnapshotId = await _dbContext.Snapshots
                .OrderByDescending(s => s.AcquiredAt)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (latestSnapshotId == 0)
            {
                return new ComputedMarketDepthResult();
            }

            var bids = await _dbContext.Bids
                .Where(b => b.OrderBookSnapshotId == latestSnapshotId)
                .Select(b => new[] { b.Price.ToString(), b.Amount.ToString() })
                .ToListAsync();

            var asks = await _dbContext.Asks
                .Where(a => a.OrderBookSnapshotId == latestSnapshotId)
                .Select(a => new[] { a.Price.ToString(), a.Amount.ToString() })
                .ToListAsync();


            return ComputeDepthChartData(bids, asks);
        }

        private static ComputedMarketDepthResult ComputeDepthChartData(IEnumerable<string[]> bids, IEnumerable<string[]> asks)
        {
            var bidOrders = bids.Select(b => new ApiOrder
            {
                Price = double.TryParse(b[0], out double bp) ? bp : 0,
                Quantity = double.TryParse(b[1], out double bq) ? bq : 0
            }).ToList();

            var askOrders = asks.Select(a => new ApiOrder
            {
                Price = double.TryParse(a[0], out double ap) ? ap : 0,
                Quantity = double.TryParse(a[1], out double aq) ? aq : 0
            }).ToList();

            bidOrders.Sort((x, y) => y.Price.CompareTo(x.Price));
            askOrders.Sort((x, y) => x.Price.CompareTo(y.Price));

            double cumulative = 0;
            var bidsDepth = bidOrders.Select(b =>
            {
                cumulative += b.Quantity;
                return new DepthChartPoint { Price = b.Price, BidsDepth = cumulative };
            }).ToList();

            cumulative = 0;
            var asksDepth = askOrders.Select(a =>
            {
                cumulative += a.Quantity;
                return new DepthChartPoint { Price = a.Price, AsksDepth = cumulative };
            }).ToList();

            // Merge bids and asks by price
            var allPrices = new HashSet<double>(bidsDepth.Select(b => b.Price).Concat(asksDepth.Select(a => a.Price)));
            var merged = allPrices.Select(price =>
            {
                var bidPoint = bidsDepth.FirstOrDefault(b => b.Price == price);
                var askPoint = asksDepth.FirstOrDefault(a => a.Price == price);
                return new DepthChartPoint
                {
                    Price = price,
                    BidsDepth = bidPoint?.BidsDepth,
                    AsksDepth = askPoint?.AsksDepth
                };
            }).OrderBy(p => p.Price).ToList();

            return new ComputedMarketDepthResult { Data = merged };
        }
        //Data trim logic removed as this exists on UI
        // Compute midPrice from best bid and best ask
        //double bestBid = bidsDepth.Count > 0 ? bidsDepth.Max(b => b.Price) : 0;
        //double bestAsk = asksDepth.Count > 0 ? asksDepth.Min(a => a.Price) : 0;
        //    if (bestBid == 0 || bestAsk == 0)
        //{
        //    // If no valid data, return what we have (could be empty)
        //    return new ComputedMarketDepthResult { Data = merged
        //    };
        //}
        //double midPrice = (bestBid + bestAsk) / 2.0;
        //double rangePercent = 0.05; // 5% range
        //double lowerBound = midPrice * (1 - rangePercent);
        //double upperBound = midPrice * (1 + rangePercent);

        //var trimmed = merged.Where(p => p.Price >= lowerBound && p.Price <= upperBound).ToList();
    }
}
