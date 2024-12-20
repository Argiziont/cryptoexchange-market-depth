using CryptoexchangeMarketDepth.Shared;
using Microsoft.EntityFrameworkCore;

namespace CryptoexchangeMarketDepth.Infrastructure.Persistence
{
    public class OrderBookDbContext : DbContext
    {
        public OrderBookDbContext(DbContextOptions<OrderBookDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrderBookSnapshot> Snapshots { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Ask> Asks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderBookSnapshot>()
                .HasIndex(s => s.AcquiredAt)
                .HasDatabaseName("IX_Snapshots_AcquiredAt");

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Snapshot)
                .WithMany(s => s.Bids)
                .HasForeignKey(b => b.OrderBookSnapshotId);

            modelBuilder.Entity<Ask>()
                .HasOne(a => a.Snapshot)
                .WithMany(s => s.Asks)
                .HasForeignKey(a => a.OrderBookSnapshotId);
        }
    }

}
