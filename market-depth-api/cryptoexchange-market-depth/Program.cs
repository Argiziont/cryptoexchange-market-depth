using CryptoexchangeMarketDepth.Clients.Integrations;
using CryptoexchangeMarketDepth.Context;
using CryptoexchangeMarketDepth.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CryptoexchangeMarketDepth.Services.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<BitstampApiOptions>(builder.Configuration.GetSection("BitstampApi"));
builder.Services.Configure<FetcherServiceOptions>(builder.Configuration.GetSection("FetcherService"));

builder.Services.AddHttpClient<BitstampApiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<BitstampApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddDbContext<OrderBookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true)
            .AllowAnyHeader());
});

builder.Services.AddHostedService<DataFetcherService>();
builder.Services.AddHostedService<DataPrunerService>();

builder.Services.AddScoped<MarketDepthComputer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseCors("CorsPolicy");

// Map the SignalR Hub endpoint
app.MapHub<MarketDepthHub>("/marketdepthhub");


app.Run();