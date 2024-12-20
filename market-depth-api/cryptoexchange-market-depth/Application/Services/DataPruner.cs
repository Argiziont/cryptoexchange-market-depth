using CryptoexchangeMarketDepth.Application.Interfaces;
using CryptoexchangeMarketDepth.Application.Options;
using CryptoexchangeMarketDepth.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace CryptoexchangeMarketDepth.Application.Services
{
    public class DataPruner : IDataPruner
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FetcherServiceOptions _options;

        public DataPruner(IServiceProvider serviceProvider, IOptions<FetcherServiceOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        public async Task PruneOldDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderBookDbContext>();

            var cutoff = DateTime.UtcNow.AddSeconds(-_options.DataLiveTime);
            var oldSnapshots = dbContext.Snapshots.Where(s => s.AcquiredAt < cutoff);

            dbContext.Snapshots.RemoveRange(oldSnapshots);
            await dbContext.SaveChangesAsync();
        }
    }
}