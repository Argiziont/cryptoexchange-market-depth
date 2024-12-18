using Microsoft.AspNetCore.SignalR;

namespace CryptoexchangeMarketDepth.Services
{
    public class MarketDepthHub : Hub
    {
        // The hub can remain empty if broadcasting from other services.
        // Clients connect to /marketdepthhub to receive messages.
    }
}