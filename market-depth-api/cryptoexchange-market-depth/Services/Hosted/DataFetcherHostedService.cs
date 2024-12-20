using CryptoexchangeMarketDepth.Services.Interfaces;
using Microsoft.Extensions.Options;
using CryptoexchangeMarketDepth.Services.Options;

namespace CryptoexchangeMarketDepth.Services.Hosted
{
    public class DataFetcherHostedService : BackgroundService
    {
        private readonly ILogger<DataFetcherHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly FetcherServiceOptions _options;

        public DataFetcherHostedService(
            ILogger<DataFetcherHostedService> logger,
            IServiceProvider serviceProvider,
            IOptions<FetcherServiceOptions> options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataFetcherHostedService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dataFetcher = scope.ServiceProvider.GetRequiredService<IDataFetcher>();
                        await dataFetcher.FetchAndStoreDataAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while fetching and storing data.");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.FetchDelay), stoppingToken);
            }

            _logger.LogInformation("DataFetcherHostedService is stopping.");
        }
    }
}