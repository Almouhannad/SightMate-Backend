using Moq;
using Moq.Protected;
using OCRService.Domain.Entities.Input;
using OCRService.Domain.Entities.Input.Options;
using OCRService.Infrastructure.OCR;
using SharedKernel.Multimedia;
using System.Net;
using System.Text;
using System.Text.Json;
using static OCROutputDTO;

namespace OCRService.Tests.Infrastructure.OCR
{
    public class OCRServiceProviderTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly OCRServiceProvider _ocrServiceProvider;
        private readonly Uri _endpointUri = new("http://mock_endpoint/");

        public OCRServiceProviderTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = _endpointUri
            };
            Environment.SetEnvironmentVariable("OCR_SERVICE_BASE_URI", _endpointUri.OriginalString);
            Environment.SetEnvironmentVariable("OCR_SERVICE_API_KEY", "Hello, I'm a mock key");

            _ocrServiceProvider = new OCRServiceProvider(_httpClient);
        }

        [Fact]
        public async Task IsAvailable_ServiceReturnsOk_ReturnsTrue()
        {
            // Arrange
            var responseContent = "{\"status\": \"ok\"}";
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri == new Uri(_endpointUri, "health")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var result = await _ocrServiceProvider.IsAvailable();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsAvailable_ServiceReturnsNotOk_ReturnsFalse()
        {
            // Arrange
            var responseContent = "{\"status\": \"error\"}";
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri == new Uri(_endpointUri, "health")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var result = await _ocrServiceProvider.IsAvailable();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsAvailable_ServiceReturnsNonSuccessStatusCode_ReturnsFalse()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });


            // Act
            var result = await _ocrServiceProvider.IsAvailable();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsAvailable_HttpRequestThrowsException_ReturnsFalse()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());

            // Act
            var result = await _ocrServiceProvider.IsAvailable();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ProcessOCR_ServiceReturnsSuccess_ReturnsSuccessResult()
        {
            // Arrange
            var ocrInput = new OCRInput
            {
                Image = new Image { Bytes = new List<int>([1,2,3]), Metadata = new Dictionary<string, object> ()},
                Options = new OCROptions()
            };
            var ocrOutputDto = new OCROutputDTO { Texts = Array.Empty<OcrTextBlockDto>() }; // Example DTO
            var jsonResponse = JsonSerializer.Serialize(ocrOutputDto);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _ocrServiceProvider.ProcessOCR(ocrInput);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task ProcessOCR_ServiceReturnsNonSuccessStatusCode_ReturnsFailureResult()
        {
            // Arrange
            var ocrInput = new OCRInput
            {
                Image = new Image { Bytes = new List<int>([1, 2, 3]), Metadata = new Dictionary<string, object>() },
                Options = new OCROptions()
            };
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act
            var result = await _ocrServiceProvider.ProcessOCR(ocrInput);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("OCR.SERVICE_ERROR", result.Error.Code);
        }

        [Fact]
        public async Task ProcessOCR_HttpRequestThrowsException_ReturnsFailureResult()
        {
            // Arrange
            var ocrInput = new OCRInput
            {
                Image = new Image { Bytes = new List<int>([1, 2, 3]), Metadata = new Dictionary<string, object>() },
                Options = new OCROptions()
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());

            // Act
            var result = await _ocrServiceProvider.ProcessOCR(ocrInput);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("OCR.SERVICE_ERROR", result.Error.Code);
        }

        [Fact]
        public async Task ProcessOCR_InvalidJsonResponse_ReturnsFailureResult()
        {
            // Arrange
            var ocrInput = new OCRInput
            {
                Image = new Image { Bytes = new List<int>([1, 2, 3]), Metadata = new Dictionary<string, object>() },
                Options = new OCROptions()
            };
            var invalidJsonResponse = "This is not valid JSON";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(invalidJsonResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _ocrServiceProvider.ProcessOCR(ocrInput);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("OCR.SERVICE_ERROR", result.Error.Code);
        }
    }
}
