using CryptoexchangeMarketDepth.Clients.Integrations;
using CryptoexchangeMarketDepth.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ErrorResponse = CryptoexchangeMarketDepth.Models.ErrorResponse;

namespace CryptoexchangeMarketDepth.Tests.Controllers
{
    public class BaseControllerTests
    {
        private readonly Mock<ILogger<BaseController>> _mockLogger;
        private readonly Mock<IBitstampApiClient> _mockBitstampClient;
        private readonly BaseController _controller;

        public BaseControllerTests()
        {
            _mockLogger = new Mock<ILogger<BaseController>>();
            _mockBitstampClient = new Mock<IBitstampApiClient>(MockBehavior.Strict);
            _controller = new BaseController(_mockLogger.Object, _mockBitstampClient.Object);
        }

        [Fact]
        public async Task GetOrderBook_ReturnsOkResult_WithOrderBookResponse()
        {
            // Arrange
            string marketSymbol = "btceur";
            var expectedOrderBookResponse = new OrderBookResponse
            {
                Asks = new List<List<string>>
                {
                    new List<string> { "50100", "0.1" },
                    new List<string> { "50200", "0.2" }
                },
                Bids = new List<List<string>>
                {
                    new List<string> { "50000", "0.1" },
                    new List<string> { "49900", "0.2" }
                },
                Microtimestamp = "1633072800000",
                Timestamp = "1633072800"
            };

            _mockBitstampClient
                .Setup(client => client.GetOrderBookAsync(marketSymbol))
                .ReturnsAsync(expectedOrderBookResponse);

            // Act
            var result = await _controller.GetOrderBook(marketSymbol);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OrderBookResponse>(okResult.Value);
            Assert.Equal(expectedOrderBookResponse.Asks.Count, returnValue.Asks.Count);
            Assert.Equal(expectedOrderBookResponse.Bids.Count, returnValue.Bids.Count);
            Assert.Equal(expectedOrderBookResponse.Timestamp, returnValue.Timestamp);
            Assert.Equal(expectedOrderBookResponse.Microtimestamp, returnValue.Microtimestamp);

            _mockBitstampClient.Verify(client => client.GetOrderBookAsync(marketSymbol), Times.Once);
        }

        [Fact]
        public async Task GetOrderBook_ReturnsBadRequest_WithErrorResponse_WhenExceptionThrown()
        {
            // Arrange
            string marketSymbol = "btceur";
            var exceptionMessage = "API error";

            _mockBitstampClient
                .Setup(client => client.GetOrderBookAsync(marketSymbol))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetOrderBook(marketSymbol);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal(exceptionMessage, errorResponse.Message);

            _mockBitstampClient.Verify(client => client.GetOrderBookAsync(marketSymbol), Times.Once);
        }

        [Fact]
        public async Task GetOrderBook_UsesDefaultMarketSymbol_WhenNotProvided()
        {
            // Arrange
            string defaultMarketSymbol = "btceur";
            var expectedOrderBookResponse = new OrderBookResponse
            {
                Asks = new List<List<string>>
                {
                    new List<string> { "50100", "0.1" },
                    new List<string> { "50200", "0.2" }
                },
                Bids = new List<List<string>>
                {
                    new List<string> { "50000", "0.1" },
                    new List<string> { "49900", "0.2" }
                },
                Microtimestamp = "1633072800000",
                Timestamp = "1633072800"
            };

            _mockBitstampClient
                .Setup(client => client.GetOrderBookAsync(defaultMarketSymbol))
                .ReturnsAsync(expectedOrderBookResponse);

            // Act
            var result = await _controller.GetOrderBook();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OrderBookResponse>(okResult.Value);
            Assert.Equal(expectedOrderBookResponse.Asks.Count, returnValue.Asks.Count);
            Assert.Equal(expectedOrderBookResponse.Bids.Count, returnValue.Bids.Count);
            Assert.Equal(expectedOrderBookResponse.Timestamp, returnValue.Timestamp);
            Assert.Equal(expectedOrderBookResponse.Microtimestamp, returnValue.Microtimestamp);

            _mockBitstampClient.Verify(client => client.GetOrderBookAsync(defaultMarketSymbol), Times.Once);
        }
    }
}
