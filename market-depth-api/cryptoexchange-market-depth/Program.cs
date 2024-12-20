using CryptoexchangeMarketDepth.Clients.Integrations;
using CryptoexchangeMarketDepth.Context;
using CryptoexchangeMarketDepth.Services;
using CryptoexchangeMarketDepth.Services.Hosted;
using CryptoexchangeMarketDepth.Services.Interfaces;
using CryptoexchangeMarketDepth.Services.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<BitstampApiOptions>(builder.Configuration.GetSection("BitstampApi"));
builder.Services.Configure<FetcherServiceOptions>(builder.Configuration.GetSection("FetcherService"));

builder.Services.AddHttpClient<IBitstampApiClient, BitstampApiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<BitstampApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddDbContext<OrderBookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true)
            .AllowAnyHeader());
});

// Register dedicated services as Scoped
builder.Services.AddScoped<IMarketDepthService, MarketDepthService>();
builder.Services.AddScoped<IDataFetcher, DataFetcher>();
builder.Services.AddScoped<IDataPruner, DataPruner>();

// Register hosted services
builder.Services.AddHostedService<DataFetcherHostedService>();
builder.Services.AddHostedService<DataPrunerHostedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("CorsPolicy");

// Map the SignalR Hub endpoint
app.MapHub<MarketDepthHub>("/marketdepthhub");
app.MapControllers();

app.Run();
