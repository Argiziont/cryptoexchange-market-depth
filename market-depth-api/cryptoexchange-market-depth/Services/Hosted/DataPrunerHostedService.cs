using CryptoexchangeMarketDepth.Services.Interfaces;
using Microsoft.Extensions.Options;
using CryptoexchangeMarketDepth.Services.Options;

namespace CryptoexchangeMarketDepth.Services.Hosted
{
    public class DataPrunerHostedService : BackgroundService
    {
        private readonly ILogger<DataPrunerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly FetcherServiceOptions _options;

        public DataPrunerHostedService(
            ILogger<DataPrunerHostedService> logger,
            IServiceProvider serviceProvider,
            IOptions<FetcherServiceOptions> options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataPrunerHostedService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dataPruner = scope.ServiceProvider.GetRequiredService<IDataPruner>();
                        await dataPruner.PruneOldDataAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while pruning old data.");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.PrunerDelay), stoppingToken);
            }

            _logger.LogInformation("DataPrunerHostedService is stopping.");
        }
    }
}