using CryptoexchangeMarketDepth.Context;
using CryptoexchangeMarketDepth.Models;
using CryptoexchangeMarketDepth.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CryptoexchangeMarketDepth.Tests.Services
{
    public class MarketDepthComputerTests : IDisposable
    {
        private readonly OrderBookDbContext _dbContext;
        private readonly MarketDepthComputer _computer;

        public MarketDepthComputerTests()
        {
            var options = new DbContextOptionsBuilder<OrderBookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new OrderBookDbContext(options);

            // Seed the in-memory database with test data
            SeedDatabase();

            _computer = new MarketDepthComputer(_dbContext);
        }

        private void SeedDatabase()
        {
            var snapshot = new OrderBookSnapshot
            {
                Id = 1,
                MarketSymbol = "btceur",
                AcquiredAt = DateTime.UtcNow,
                Timestamp = DateTime.UtcNow,
                Microtimestamp = DateTime.UtcNow,
                Bids = new List<Bid>
                {
                    new Bid { Id = 1, Price = "50000", Amount = "0.1", OrderBookSnapshotId = 1 },
                    new Bid { Id = 2, Price = "49900", Amount = "0.2", OrderBookSnapshotId = 1 },
                    new Bid { Id = 3, Price = "49800", Amount = "0.3", OrderBookSnapshotId = 1 }
                },
                Asks = new List<Ask>
                {
                    new Ask { Id = 4, Price = "50100", Amount = "0.1", OrderBookSnapshotId = 1 },
                    new Ask { Id = 5, Price = "50200", Amount = "0.2", OrderBookSnapshotId = 1 },
                    new Ask { Id = 6, Price = "50300", Amount = "0.3", OrderBookSnapshotId = 1 }
                }
            };

            _dbContext.Snapshots.Add(snapshot);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task ComputeMarketDepthAsync_ReturnsCorrectDepth()
        {
            // Act
            var result = await _computer.ComputeMarketDepthAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Data);

            var data = result.Data;

            // Check bid depths
            var bid50000 = data.Find(p => p.Price == 50000);
            Assert.NotNull(bid50000);
            Assert.True(bid50000.BidsDepth.HasValue, "BidsDepth for price 50000 should have a value.");
            Assert.Equal(0.1, bid50000.BidsDepth.Value, 5);
            Assert.Null(bid50000.AsksDepth);

            var bid49900 = data.Find(p => p.Price == 49900);
            Assert.NotNull(bid49900);
            Assert.True(bid49900.BidsDepth.HasValue, "BidsDepth for price 49900 should have a value.");
            Assert.Equal(0.3, bid49900.BidsDepth.Value, 5);
            Assert.Null(bid49900.AsksDepth);

            var bid49800 = data.Find(p => p.Price == 49800);
            Assert.NotNull(bid49800);
            Assert.True(bid49800.BidsDepth.HasValue, "BidsDepth for price 49800 should have a value.");
            Assert.Equal(0.6, bid49800.BidsDepth.Value, 5);
            Assert.Null(bid49800.AsksDepth);

            // Check ask depths
            var ask50100 = data.Find(p => p.Price == 50100);
            Assert.NotNull(ask50100);
            Assert.Null(ask50100.BidsDepth);
            Assert.True(ask50100.AsksDepth.HasValue, "AsksDepth for price 50100 should have a value.");
            Assert.Equal(0.1, ask50100.AsksDepth.Value, 5);

            var ask50200 = data.Find(p => p.Price == 50200);
            Assert.NotNull(ask50200);
            Assert.Null(ask50200.BidsDepth);
            Assert.True(ask50200.AsksDepth.HasValue, "AsksDepth for price 50200 should have a value.");
            Assert.Equal(0.3, ask50200.AsksDepth.Value, 5);

            var ask50300 = data.Find(p => p.Price == 50300);
            Assert.NotNull(ask50300);
            Assert.Null(ask50300.BidsDepth);
            Assert.True(ask50300.AsksDepth.HasValue, "AsksDepth for price 50300 should have a value.");
            Assert.Equal(0.6, ask50300.AsksDepth.Value, 5);
        }

        [Fact]
        public async Task ComputeMarketDepthAsync_ReturnsEmptyResult_WhenNoSnapshots()
        {
            // Arrange
            // Clear the database
            _dbContext.Snapshots.RemoveRange(_dbContext.Snapshots);
            _dbContext.SaveChanges();

            // Act
            var result = await _computer.ComputeMarketDepthAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
