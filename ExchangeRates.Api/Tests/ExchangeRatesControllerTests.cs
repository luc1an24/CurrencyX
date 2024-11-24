using ExchangeRates.Api.Controllers;
using ExchangeRates.Api.Dto;
using ExchangeRates.Api.Interfaces;
using ExchangeRates.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ExchangeRates.Api.Tests
{
    public class ExchangeRatesControllerTests
    {
        public ExchangeRatesControllerTests()
        {
            _serviceMock = new Mock<IExchangeRateService>();
            _controller = new ExchangeRatesController(_serviceMock.Object);
        }

        private readonly ExchangeRatesController _controller;
        private readonly Mock<IExchangeRateService> _serviceMock;

        [Fact]
        public async Task GetExchangeRates_ReturnsOk_WhenDataExists()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var rates = new PagedResponse<ExchangeRateDto>(
                new List<ExchangeRateDto> { new ExchangeRateDto { CurrencyCode = "USD", Rate = 1.1 } },
                page, pageSize, 1
            );
            _serviceMock.Setup(s => s.GetExchangeRatesAsync(page, pageSize))
                .ReturnsAsync(rates);

            // Act
            var result = await _controller.GetExchangeRates(page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.IsAssignableFrom<PagedResponse<ExchangeRateDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetCalculatedExchangeRate_ReturnsBadRequest_WhenInvalidInput()
        {
            // Arrange

            // Act
            var result = await _controller.GetCalculatedExchangeRate("", -1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
