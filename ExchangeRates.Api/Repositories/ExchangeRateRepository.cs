using ExchangeRates.Api.Interfaces;
using ExchangeRates.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRates.Api.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        public ExchangeRateRepository(ExchangeRatesDbContext context)
        {
            _context = context;
        }

        private readonly ExchangeRatesDbContext _context;

        public async Task<int> GetTotalExchangeRateCountAsync()
        {
            return await _context.ExchangeRates.CountAsync();

        }
        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesPagedAsync(int page, int pageSize)
        {
            return await _context.ExchangeRates
                .OrderByDescending(e => e.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ExchangeRate?> GetLatestExchangeRateAsync(string currencyCode)
        {
            return await _context.ExchangeRates
                .Where(e => e.CurrencyCode == currencyCode.ToUpperInvariant())
                .OrderByDescending(e => e.Date)
                .FirstOrDefaultAsync();
        }

        public async Task SaveExchangeRateAsync(ExchangeRate exchangeRate)
        {
            var exists = await _context.ExchangeRates
                .AnyAsync(e => e.CurrencyCode == exchangeRate.CurrencyCode && e.Date == exchangeRate.Date);

            if (!exists)
            {
                _context.ExchangeRates.Add(exchangeRate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExchangeRateAsync(string currencyCode)
        {
            var latestExchangeRate = await _context.ExchangeRates
                .Where(e => e.CurrencyCode == currencyCode.ToUpperInvariant())
                .OrderByDescending(e => e.Date)
                .FirstOrDefaultAsync();

            if (latestExchangeRate == null)
                throw new KeyNotFoundException($"Exchange rate for currency '{currencyCode}' not found.");

            _context.ExchangeRates.Remove(latestExchangeRate);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExchangeRateAsync(ExchangeRate exchangeRate)
        {
            var existingRate = await _context.ExchangeRates
                .FirstOrDefaultAsync(e => e.CurrencyCode == exchangeRate.CurrencyCode && e.Date == exchangeRate.Date);

            if (existingRate == null)
            {
                throw new KeyNotFoundException($"Exchange rate for {exchangeRate.CurrencyCode} on {exchangeRate.Date} not found.");
            }

            existingRate.Rate = exchangeRate.Rate;

            _context.ExchangeRates.Update(existingRate);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExchangeRate>> SearchExchangeRatesAsync(string? currencyCode, DateTime? date)
        {
            var query = _context.ExchangeRates.AsQueryable();

            if (!string.IsNullOrEmpty(currencyCode))
            {
                query = query.Where(e => e.CurrencyCode == currencyCode.ToUpperInvariant());
            }

            if (date.HasValue)
            {
                query = query.Where(e => e.Date.Date == date.Value.Date);
            }

            return await query.ToListAsync();
        }
    }
}
