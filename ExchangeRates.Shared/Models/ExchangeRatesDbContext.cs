using Microsoft.EntityFrameworkCore;

namespace ExchangeRates.Shared.Models
{
    public class ExchangeRatesDbContext : DbContext
    {
        public ExchangeRatesDbContext(DbContextOptions<ExchangeRatesDbContext> options) : base(options) { }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>()
                .HasKey(e => new { e.CurrencyCode, e.Date });

            modelBuilder.Entity<ExchangeRate>()
                .Property(e => e.CurrencyCode)
                .IsRequired();

            modelBuilder.Entity<ExchangeRate>()
                .Property(e => e.Date)
                .IsRequired();
        }
    }
}
