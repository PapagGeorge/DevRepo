using Microsoft.EntityFrameworkCore;
using WalletCore.Domain.DBModels;

namespace WalletCore.Infrastructure
{
    public class ExchangeRateDbContext : DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        public ExchangeRateDbContext(DbContextOptions<ExchangeRateDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>()
                .HasIndex(x => new { x.Date, x.CurrencyCode })
                .IsUnique();
        }
    }
}
