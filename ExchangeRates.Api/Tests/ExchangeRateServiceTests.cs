using AutoMapper;
using ExchangeRates.Api.Dto;
using ExchangeRates.Api.Interfaces;
using ExchangeRates.Api.Services;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using Moq;
using Xunit;

namespace ExchangeRates.Api.Tests
{
    public class ExchangeRateServiceTests
    {
        public ExchangeRateServiceTests()
        {
            _repositoryMock = new Mock<IExchangeRateRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _externalApiOptionsMock = new Mock<IExternalApiOptions>();
            _httpClientMock = new Mock<HttpClient>();
            _mapperMock = new Mock<IMapper>();

            _service = new ExchangeRateService(
                _repositoryMock.Object,
                _cacheServiceMock.Object,
                _externalApiOptionsMock.Object,
                _httpClientMock.Object,
                _mapperMock.Object
            );
        }

        private readonly ExchangeRateService _service;
        private readonly Mock<IExchangeRateRepository> _repositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IExternalApiOptions> _externalApiOptionsMock;
        private readonly Mock<HttpClient> _httpClientMock;
        private readonly Mock<IMapper> _mapperMock;

        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsPagedResponse_WhenDataExists()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var exchangeRates = new List<ExchangeRate>
        {
            new ExchangeRate { CurrencyCode = "USD", Rate = 1.1, Date = DateTime.UtcNow }
        };
            _repositoryMock.Setup(repo => repo.GetTotalExchangeRateCountAsync())
                .ReturnsAsync(1);
            _repositoryMock.Setup(repo => repo.GetExchangeRatesPagedAsync(page, pageSize))
                .ReturnsAsync(exchangeRates);

            // Act
            var result = await _service.GetExchangeRatesAsync(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalRecords);
            Assert.Single(result.Data);
            _repositoryMock.Verify(repo => repo.GetExchangeRatesPagedAsync(page, pageSize), Times.Once);
        }

        [Fact]
        public async Task GetCalculatedExchangeRate_ThrowsInvalidOperationException_WhenRateNotFound()
        {
            // Arrange
            _repositoryMock.Setup(repo => repo.GetLatestExchangeRateAsync("USD"))
                .ReturnsAsync((ExchangeRate)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetCalculatedExchangeRate("USD", 100));
        }

        [Fact]
        public async Task DeleteLatestExchangeRate_ThrowsArgumentException_WhenCurrencyCodeIsEmpty()
        {
            // Arrange
            string invalidCurrencyCode = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.DeleteLatestExchangeRate(invalidCurrencyCode)
            );

            Assert.Equal("Currency code cannot be null or empty. (Parameter 'currencyCode')", exception.Message);
        }

        [Fact]
        public async Task CreateExchangeRateAsync_CreatesExchangeRateAndInvalidatesCache()
        {
            // Arrange
            var dto = new ExchangeRateCreateDto
            {
                CurrencyCode = "USD",
                Rate = 1.23
            };

            var exchangeRate = new ExchangeRate
            {
                CurrencyCode = "USD",
                Rate = 1.23,
                Date = DateTime.UtcNow
            };

            _mapperMock.Setup(m => m.Map<ExchangeRate>(dto)).Returns(exchangeRate);
            _repositoryMock.Setup(r => r.SaveExchangeRateAsync(exchangeRate)).Returns(Task.CompletedTask);
            _cacheServiceMock.Setup(c => c.RemoveByPatternAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            await _service.CreateExchangeRateAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.SaveExchangeRateAsync(exchangeRate), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveByPatternAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsCachedData_WhenCacheExists()
        {
            // Arrange
            var cachedResponse = new PagedResponse<ExchangeRateDto>(
                new List<ExchangeRateDto>
                {
                new ExchangeRateDto { CurrencyCode = "USD", Rate = 1.1 }
                },
                1, 10, 1
            );

            _cacheServiceMock.Setup(c => c.GetAsync<PagedResponse<ExchangeRateDto>>(It.IsAny<string>()))
                .ReturnsAsync(cachedResponse);

            // Act
            var result = await _service.GetExchangeRatesAsync(1, 10);

            // Assert
            Assert.Equal(1, result.TotalRecords);
            Assert.Single(result.Data);
            _repositoryMock.Verify(r => r.GetExchangeRatesPagedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsDataFromRepository_WhenCacheIsEmpty()
        {
            // Arrange
            var rates = new List<ExchangeRate>
        {
            new ExchangeRate { CurrencyCode = "USD", Rate = 1.1, Date = DateTime.UtcNow }
        };
            _repositoryMock.Setup(r => r.GetExchangeRatesPagedAsync(1, 10)).ReturnsAsync(rates);
            _repositoryMock.Setup(r => r.GetTotalExchangeRateCountAsync()).ReturnsAsync(1);
            _cacheServiceMock.Setup(c => c.GetAsync<PagedResponse<ExchangeRateDto>>(It.IsAny<string>())).ReturnsAsync((PagedResponse<ExchangeRateDto>)null);

            // Act
            var result = await _service.GetExchangeRatesAsync(1, 10);

            // Assert
            Assert.Single(result.Data);
            _repositoryMock.Verify(r => r.GetExchangeRatesPagedAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetCalculatedExchangeRate_ReturnsCorrectCalculation()
        {
            // Arrange
            var rate = new ExchangeRate { CurrencyCode = "USD", Rate = 1.2 };
            _repositoryMock.Setup(r => r.GetLatestExchangeRateAsync("USD")).ReturnsAsync(rate);

            // Act
            var result = await _service.GetCalculatedExchangeRate("USD", 100);

            // Assert
            Assert.Equal(120, result);
        }

        [Fact]
        public async Task UpdateExchangeRateAsync_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            var dto = new ExchangeRateUpdateDto { Rate = 1.5 };
            var rate = new ExchangeRate { CurrencyCode = "USD", Rate = 1.5 };

            _mapperMock.Setup(m => m.Map<ExchangeRate>(dto)).Returns(rate);
            _repositoryMock.Setup(r => r.UpdateExchangeRateAsync(rate)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateExchangeRateAsync("USD", dto);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.UpdateExchangeRateAsync(rate), Times.Once);
        }
    }
}
