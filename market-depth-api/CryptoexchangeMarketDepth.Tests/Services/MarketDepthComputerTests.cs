using CryptoexchangeMarketDepth.Context;
using CryptoexchangeMarketDepth.Models;
using CryptoexchangeMarketDepth.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using CryptoexchangeMarketDepth.Services.Models;

namespace CryptoexchangeMarketDepth.Tests.Services
{
    public class MarketDepthHubTests : IDisposable
    {
        private readonly OrderBookDbContext _dbContext;
        private readonly MarketDepthHub _hub;
        private readonly Mock<HubCallerContext> _mockContext;
        private readonly Mock<ISingleClientProxy> _mockClientProxy;
        private readonly Mock<IHubCallerClients> _mockClients;

        public MarketDepthHubTests()
        {
            var options = new DbContextOptionsBuilder<OrderBookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new OrderBookDbContext(options);
            SeedDatabase();

            _hub = new MarketDepthHub(_dbContext);

            _mockContext = new Mock<HubCallerContext>();
            _hub.Context = _mockContext.Object;

            _mockClientProxy = new Mock<ISingleClientProxy>();
            _mockClients = new Mock<IHubCallerClients>();
            _mockClients.Setup(clients => clients.Caller).Returns(_mockClientProxy.Object);
            _hub.Clients = _mockClients.Object;
        }

        private void SeedDatabase()
        {
            var snapshots = new List<OrderBookSnapshot>
            {
                new OrderBookSnapshot
                {
                    Id = 1,
                    MarketSymbol = "btceur",
                    AcquiredAt = DateTime.UtcNow.AddMinutes(-15),
                    Timestamp = DateTime.UtcNow.AddMinutes(-15),
                    Microtimestamp = DateTime.UtcNow.AddMinutes(-15),
                    Bids = new List<Bid>
                    {
                        new Bid { Id = 1, Price = "50000", Amount = "0.1", OrderBookSnapshotId = 1 },
                        new Bid { Id = 2, Price = "49900", Amount = "0.2", OrderBookSnapshotId = 1 }
                    },
                    Asks = new List<Ask>
                    {
                        new Ask { Id = 3, Price = "50100", Amount = "0.1", OrderBookSnapshotId = 1 },
                        new Ask { Id = 4, Price = "50200", Amount = "0.2", OrderBookSnapshotId = 1 }
                    }
                },
                new OrderBookSnapshot
                {
                    Id = 2,
                    MarketSymbol = "btceur",
                    AcquiredAt = DateTime.UtcNow.AddMinutes(-14),
                    Timestamp = DateTime.UtcNow.AddMinutes(-14),
                    Microtimestamp = DateTime.UtcNow.AddMinutes(-14),
                    Bids = new List<Bid>
                    {
                        new Bid { Id = 5, Price = "50010", Amount = "0.15", OrderBookSnapshotId = 2 },
                        new Bid { Id = 6, Price = "49910", Amount = "0.25", OrderBookSnapshotId = 2 }
                    },
                    Asks = new List<Ask>
                    {
                        new Ask { Id = 7, Price = "50110", Amount = "0.15", OrderBookSnapshotId = 2 },
                        new Ask { Id = 8, Price = "50210", Amount = "0.25", OrderBookSnapshotId = 2 }
                    }
                },
            };

            for (int i = 3; i <= 15; i++)
            {
                snapshots.Add(new OrderBookSnapshot
                {
                    Id = i,
                    MarketSymbol = "btceur",
                    AcquiredAt = DateTime.UtcNow.AddMinutes(-15 + i),
                    Timestamp = DateTime.UtcNow.AddMinutes(-15 + i),
                    Microtimestamp = DateTime.UtcNow.AddMinutes(-15 + i),
                    Bids = new List<Bid>
                    {
                        new Bid { Id = i * 2 + 1, Price = (50000 + i).ToString(), Amount = (0.1 + i * 0.01).ToString(), OrderBookSnapshotId = i },
                        new Bid { Id = i * 2 + 2, Price = (49900 + i).ToString(), Amount = (0.2 + i * 0.01).ToString(), OrderBookSnapshotId = i }
                    },
                    Asks = new List<Ask>
                    {
                        new Ask { Id = i * 2 + 3, Price = (50100 + i).ToString(), Amount = (0.1 + i * 0.01).ToString(), OrderBookSnapshotId = i },
                        new Ask { Id = i * 2 + 4, Price = (50200 + i).ToString(), Amount = (0.2 + i * 0.01).ToString(), OrderBookSnapshotId = i }
                    }
                });
            }

            _dbContext.Snapshots.AddRange(snapshots);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetLastSnapshotsAsync_ReturnsLastTenSnapshots()
        {
            // Act
            var lastSnapshots = await _hub.GetLastSnapshotsAsync();

            // Assert
            Assert.Equal(10, lastSnapshots.Count);
            Assert.Equal(15, lastSnapshots[0].Id);
            Assert.Equal(6, lastSnapshots[9].Id);
        }

        [Fact]
        public async Task OnConnectedAsync_SendsLastSnapshots()
        {
            // Act
            await _hub.OnConnectedAsync();

            // Assert
            _mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveLastSnapshots",
                    It.Is<object[]>(o => o != null && o.Length == 1 && o[0] is List<Snapshot>),
                    default),
                Times.Once);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
