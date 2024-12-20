using CryptoexchangeMarketDepth.Api.Hubs;
using CryptoexchangeMarketDepth.Application.DTOs;
using CryptoexchangeMarketDepth.Application.Interfaces;
using CryptoexchangeMarketDepth.Application.Options;
using CryptoexchangeMarketDepth.Infrastructure.ExternalServices.Integrations;
using CryptoexchangeMarketDepth.Infrastructure.Persistence;
using CryptoexchangeMarketDepth.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CryptoexchangeMarketDepth.Application.Services
{
    public class DataFetcher : IDataFetcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataFetcher> _logger;
        private readonly FetcherServiceOptions _options;
        private readonly IMarketDepthService _marketDepthService;
        private readonly IHubContext<MarketDepthHub> _hubContext;

        public DataFetcher(
            IServiceProvider serviceProvider,
            ILogger<DataFetcher> logger,
            IOptions<FetcherServiceOptions> options,
            IMarketDepthService marketDepthService,
            IHubContext<MarketDepthHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
            _marketDepthService = marketDepthService;
            _hubContext = hubContext;
        }

        public async Task FetchAndStoreDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var bitstampClient = scope.ServiceProvider.GetRequiredService<IBitstampApiClient>();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderBookDbContext>();

            try
            {
                var response = await bitstampClient.GetOrderBookAsync(_options.MarketSymbol);
                if (response != null)
                {
                    var snapshot = new OrderBookSnapshot
                    {
                        AcquiredAt = DateTime.UtcNow,
                        MarketSymbol = _options.MarketSymbol,
                        Timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(response.Timestamp)).UtcDateTime,
                        Microtimestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(response.Microtimestamp) / 1000).UtcDateTime,
                        Bids = response.Bids.Select(b => new Bid { Price = b[0], Amount = b[1] }).ToList(),
                        Asks = response.Asks.Select(a => new Ask { Price = a[0], Amount = a[1] }).ToList()
                    };
                    dbContext.Snapshots.Add(snapshot);
                    await dbContext.SaveChangesAsync();

                    var lastSnapshots = await dbContext.Snapshots
                        .OrderByDescending(s => s.AcquiredAt)
                        .Take(10)
                        .Select(s => new Snapshot
                        {
                            Id = s.Id,
                            AcquiredAt = s.AcquiredAt,
                            Timestamp = s.Timestamp
                        }).ToListAsync();

                    var computedData = await _marketDepthService.ComputeMarketDepthAsync();

                    var rawData = new RawData
                    {
                        Asks = snapshot.Asks.Select(x => new RawApiOrder { Price = x.Price, Amount = x.Amount }).ToList(),
                        Bids = snapshot.Bids.Select(x => new RawApiOrder { Price = x.Price, Amount = x.Amount }).ToList(),
                        Timestamp = snapshot.Timestamp,
                        AcquiredAt = snapshot.AcquiredAt
                    };

                    // Broadcast to all connected clients
                    await _hubContext.Clients.All.SendAsync("UpdateDepthData", computedData);
                    await _hubContext.Clients.All.SendAsync("UpdateRawData", rawData);
                    await _hubContext.Clients.All.SendAsync("ReceiveLastSnapshots", lastSnapshots);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching or storing data");
            }
        }
    }
}