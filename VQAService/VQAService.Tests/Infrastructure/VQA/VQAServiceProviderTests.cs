using Moq;
using Moq.Protected;
using SharedKernel.Base;
using SharedKernel.Multimedia;
using System.Net;
using System.Text;
using System.Text.Json;
using VQAService.Domain.Entities.Conversations;
using VQAService.Infrastructure.VQA;
using VQAService.Infrastructure.VQA.DTOs;
using System.Text.Json.Serialization;

namespace VQAService.Tests.Infrastructure.VQA;

public class VQAServiceProviderTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly VQAServiceProvider _vqaServiceProvider;

    public VQAServiceProviderTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        // For testing, we can set a dummy base address and ensure the HttpClient mock handles it.
        _httpClient.BaseAddress = new Uri("http://dummy-vqa-service.com/");
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", "dummy_api_key");
        // Important note: Testing will run on windows, so environment variables won't be loaded
        // So, go to ServiceProvider and comment loading endpoint from config to prevent CONFIG exceptions
        _vqaServiceProvider = new VQAServiceProvider(_httpClient);
    }

    private HttpResponseMessage CreateHttpResponse(HttpStatusCode statusCode, object? content = null)
    {
        var response = new HttpResponseMessage(statusCode);
        if (content != null)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            response.Content = new StringContent(JsonSerializer.Serialize(content, options), Encoding.UTF8, "application/json");
        }
        return response;
    }

    [Fact]
    public async Task IsAvailable_ServiceReturnsOk_ReturnsTrue()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri == new Uri(_httpClient.BaseAddress!, "health")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(CreateHttpResponse(HttpStatusCode.OK, new { status = "ok" }));

        // Act
        var result = await _vqaServiceProvider.IsAvailable();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsAvailable_ServiceReturnsNotOk_ReturnsFalse()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri == new Uri(_httpClient.BaseAddress!, "health")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(CreateHttpResponse(HttpStatusCode.OK, new { status = "error" }));

        // Act
        var result = await _vqaServiceProvider.IsAvailable();

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
            .ReturnsAsync(CreateHttpResponse(HttpStatusCode.InternalServerError));

        // Act
        var result = await _vqaServiceProvider.IsAvailable();

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
        var result = await _vqaServiceProvider.IsAvailable();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ProcessIC_ServiceReturnsSuccess_ReturnsSuccessResult()
    {
        // Arrange
        var image = new Image { Bytes = new List<int> { 1, 2, 3 }, Metadata = new Dictionary<string, object>() };
        var expectedCaption = "A test caption.";
        var responseDto = new ResponseDTO { Output = expectedCaption };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri == new Uri(_httpClient.BaseAddress!, "vqa/captioning") &&
                    req.Method == HttpMethod.Post
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(CreateHttpResponse(HttpStatusCode.OK, responseDto));

        // Act
        var result = await _vqaServiceProvider.ProcessIC(image);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCaption, result.Value);
    }

    [Fact]
    public async Task ProcessIC_ServiceReturnsNonSuccessStatusCode_ReturnsFailureResult()
    {
        // Arrange
        var image = new Image { Bytes = new List<int> { 1, 2, 3 }, Metadata = new Dictionary<string, object>() };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(CreateHttpResponse(HttpStatusCode.InternalServerError));

        // Act
        var result = await _vqaServiceProvider.ProcessIC(image);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.SERVICE_ERROR", result.Error.Code);
    }

    [Fact]
    public async Task ProcessIC_HttpRequestThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var image = new Image { Bytes = new List<int> { 1, 2, 3 }, Metadata = new Dictionary<string, object>() };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());

        // Act
        var result = await _vqaServiceProvider.ProcessIC(image);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.SERVICE_ERROR", result.Error.Code);
    }

    [Fact]
    public async Task ProcessVQA_ServiceReturnsSuccess_ReturnsSuccessResult()
    {
        // Arrange
        var image = new Image { Bytes = new List<int> { 1, 2, 3 }, Metadata = new Dictionary<string, object>() };
        var question = "What is this?";
        var history = new List<HistoryItem> { new HistoryItem { Question = "PrevQ", Answer = "PrevA" } };
        var expectedAnswer = "This is a test answer.";
        var responseDto = new ResponseDTO { Output = expectedAnswer };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri == new Uri(_httpClient.BaseAddress!, "vqa/question") &&
                    req.Method == HttpMethod.Post
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(CreateHttpResponse(HttpStatusCode.OK, responseDto));

        // Act
        var result = await _vqaServiceProvider.ProcessVQA(image, question, history);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedAnswer, result.Value);
    }

    [Fact]
    public async Task ProcessVQA_ServiceReturnsNonSuccessStatusCode_ReturnsFailureResult()
    {
        // Arrange
        var image = new Image { Bytes = new List<int> { 1, 2, 3 }, Metadata = new Dictionary<string, object>() };
        var question = "What is this?";
        var history = new List<HistoryItem>();

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(CreateHttpResponse(HttpStatusCode.InternalServerError));

        // Act
        var result = await _vqaServiceProvider.ProcessVQA(image, question, history);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.SERVICE_ERROR", result.Error.Code);
    }

    [Fact]
    public async Task ProcessVQA_HttpRequestThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var image = new Image { Bytes = new List<int> { 1, 2, 3 }, Metadata = new Dictionary<string, object>() };
        var question = "What is this?";
        var history = new List<HistoryItem>();

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());

        // Act
        var result = await _vqaServiceProvider.ProcessVQA(image, question, history);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.SERVICE_ERROR", result.Error.Code);
    }
}