using CryptoexchangeMarketDepth.Clients.Integrations;
using CryptoexchangeMarketDepth.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Bitstamp API options
builder.Services.Configure<BitstampApiOptions>(
    builder.Configuration.GetSection("BitstampApi"));

// Register HttpClient for Bitstamp API
builder.Services.AddHttpClient<BitstampApiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<BitstampApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

// Register EF Core with LocalDB
builder.Services.AddDbContext<OrderBookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Background Service
//builder.Services.AddHostedService<OrderBookWorker>();

// Add Swagger and Endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();