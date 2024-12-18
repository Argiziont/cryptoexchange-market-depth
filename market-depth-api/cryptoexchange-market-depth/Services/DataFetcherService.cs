using CryptoexchangeMarketDepth.Clients.Integrations;
using CryptoexchangeMarketDepth.Context;
using CryptoexchangeMarketDepth.Models;
using CryptoexchangeMarketDepth.Services.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CryptoexchangeMarketDepth.Services.Options;
using Microsoft.EntityFrameworkCore;

namespace CryptoexchangeMarketDepth.Services
{
    public class DataFetcherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataFetcherService> _logger;
        private readonly FetcherServiceOptions _options;

        public DataFetcherService(IServiceProvider serviceProvider, ILogger<DataFetcherService> logger, IOptions<FetcherServiceOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await FetchAndStoreDataAsync();
                await Task.Delay(TimeSpan.FromSeconds(_options.FetchDelay), stoppingToken);
            }
        }

        private async Task FetchAndStoreDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var bitstampClient = scope.ServiceProvider.GetRequiredService<BitstampApiClient>();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderBookDbContext>();
            var computer = scope.ServiceProvider.GetRequiredService<MarketDepthComputer>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<MarketDepthHub>>();

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

                    var computedData = await computer.ComputeMarketDepthAsync();
                    var rawData = new RawData()
                    {
                        Asks = snapshot.Asks.Select(x => new RawApiOrder() { Price = x.Price, Amount = x.Amount }).ToList(),
                        Bids = snapshot.Bids.Select(x => new RawApiOrder() { Price = x.Price, Amount = x.Amount }).ToList(),
                        Timestamp = snapshot.Timestamp,
                        AcquiredAt = snapshot.AcquiredAt
                    };

                    // Broadcast to all connected clients
                    await hubContext.Clients.All.SendAsync("UpdateDepthData", computedData);
                    await hubContext.Clients.All.SendAsync("UpdateRawData", rawData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching or storing data");
            }
        }
    }
}
