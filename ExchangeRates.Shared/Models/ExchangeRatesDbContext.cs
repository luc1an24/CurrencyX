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
                .IsRequired()
                .HasColumnName("currencycode");

            modelBuilder.Entity<ExchangeRate>()
                .Property(e => e.Date)
                .IsRequired()
                .HasColumnName("date")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );

            modelBuilder.Entity<ExchangeRate>()
                .Property(e => e.Rate)
                .HasColumnName("rate");

            modelBuilder.Entity<ExchangeRate>().ToTable("exchangerates");
        }
    }
}
