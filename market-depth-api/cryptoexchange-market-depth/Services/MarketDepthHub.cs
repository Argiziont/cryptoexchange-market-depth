using CryptoexchangeMarketDepth.Context;
using CryptoexchangeMarketDepth.Services.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CryptoexchangeMarketDepth.Services
{
    public class MarketDepthHub : Hub
    {
        private readonly OrderBookDbContext _dbContext;

        public MarketDepthHub(OrderBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Snapshot>> GetLastSnapshotsAsync()
        {
            var snapshots = await _dbContext.Snapshots
                .OrderByDescending(s => s.AcquiredAt)
                .Take(10)
                .ToListAsync();

            return snapshots.Select(s => new Snapshot
            {
                Id = s.Id,
                AcquiredAt = s.AcquiredAt,
                Timestamp = s.Timestamp
            }).ToList();
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var lastSnapshots = await GetLastSnapshotsAsync();
            await Clients.Caller.SendAsync("ReceiveLastSnapshots", lastSnapshots);
        }
    }
}