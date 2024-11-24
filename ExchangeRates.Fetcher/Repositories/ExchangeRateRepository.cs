using ExchangeRates.Fetcher.Interfaces;
using ExchangeRates.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRates.Fetcher.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        public ExchangeRateRepository(ExchangeRatesDbContext context)
        {
            _context = context;
        }

        private readonly ExchangeRatesDbContext _context;

        public async Task SaveExchangeRateAsync(ExchangeRate exchangeRate)
        {
            var existingRate = await _context.ExchangeRates
                .FirstOrDefaultAsync(er => er.CurrencyCode == exchangeRate.CurrencyCode && er.Date == exchangeRate.Date);

            if (existingRate == null)
            {
                await _context.ExchangeRates.AddAsync(exchangeRate);
                await _context.SaveChangesAsync();
            }
        }
    }
}
