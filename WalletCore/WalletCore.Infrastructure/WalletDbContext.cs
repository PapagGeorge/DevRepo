using Microsoft.EntityFrameworkCore;
using WalletCore.Contrtacts.DBModels;

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
            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.Property(w => w.Balance)
                      .HasPrecision(18, 8); // 18 digits, 8 after decimal

                // Currency length and required
                entity.Property(w => w.Currency)
                      .IsRequired()
                      .HasMaxLength(3); // ISO currency code like "USD"

                entity.HasIndex(w => w.Currency);
            });

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
