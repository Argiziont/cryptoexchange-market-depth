using CryptoexchangeMarketDepth.Clients.Integrations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Bind configuration from appsettings.json
builder.Services.Configure<BitstampApiOptions>(
    builder.Configuration.GetSection("BitstampApi"));

// Register BitstampClient with Dependency Injection
builder.Services.AddHttpClient<BitstampApiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<BitstampApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();