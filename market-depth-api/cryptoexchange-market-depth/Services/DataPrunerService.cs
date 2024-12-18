using CryptoexchangeMarketDepth.Context;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CryptoexchangeMarketDepth.Services
{
    public class DataPrunerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FetcherServiceOptions _options;

        public DataPrunerService(IServiceProvider serviceProvider, IOptions<FetcherServiceOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PruneOldDataAsync();
                await Task.Delay(TimeSpan.FromSeconds(_options.PrunerDelay), stoppingToken);
            }
        }

        private async Task PruneOldDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderBookDbContext>();

            var cutoff = DateTime.UtcNow.AddMinutes(-1 * _options.DataLiveTime);
            var oldSnapshots = dbContext.Snapshots.Where(s => s.AcquiredAt < cutoff);

            dbContext.Snapshots.RemoveRange(oldSnapshots);
            await dbContext.SaveChangesAsync();
        }
    }
}