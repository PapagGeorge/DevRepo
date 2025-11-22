using Microsoft.EntityFrameworkCore;
using WalletCore.Domain.DBModels;

namespace WalletCore.Infrastructure
{
    public class WalletDbContext : DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        public WalletDbContext(DbContextOptions<WalletDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                // Configure Rate precision
                entity.Property(e => e.Rate)
                    .HasPrecision(18, 8); // 18 digits total, 6 after decimal

                // Unique index on Date + CurrencyCode
                entity.HasIndex(e => new { e.Date, e.CurrencyCode })
                      .IsUnique();
            });
        }
    }
}
