using Moq;
using SharedKernel.Base;
using SharedKernel.Multimedia;
using VQAService.Application.IC;
using VQAService.Domain.Entities.Conversations;
using VQAService.Domain.Interfaces;
using IdentityService.Domain.Interfaces;

namespace VQAService.Tests.Application.IC;

public class ProcessICQueryHandlerTests
{
    private readonly Mock<IVQAServiceProvider> _mockVqaServiceProvider;
    private readonly Mock<IConversationsRepository> _mockConversationsRepository;
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly ProcessICQueryHandler _handler;

    public ProcessICQueryHandlerTests()
    {
        _mockVqaServiceProvider = new Mock<IVQAServiceProvider>();
        _mockConversationsRepository = new Mock<IConversationsRepository>();
        _mockUserContext = new Mock<IUserContext>();
        _handler = new ProcessICQueryHandler(_mockVqaServiceProvider.Object, _mockConversationsRepository.Object, _mockUserContext.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResultAndCreatesConversation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(true);

        _mockVqaServiceProvider
            .Setup(s => s.ProcessIC(It.IsAny<Image>()))
            .ReturnsAsync(Result.Success("Test Caption"));

        _mockUserContext
            .Setup(u => u.UserId)
            .Returns(userId);

        _mockConversationsRepository
            .Setup(r => r.Create(It.IsAny<Conversation>()))
            .ReturnsAsync((Conversation conv) => Result.Success(new Conversation { Id = conversationId, UserId = userId, Image = conv.Image, History = conv.History }));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Caption", result.Value.Caption);
        Assert.Equal(conversationId, result.Value.ConversationId);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessIC(It.IsAny<Image>()), Times.Once);
        _mockUserContext.Verify(u => u.UserId, Times.Once);
        _mockConversationsRepository.Verify(r => r.Create(It.IsAny<Conversation>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceNotAvailable_ReturnsFailureResult()
    {
        // Arrange
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.SERVICE_NOT_AVAILABLE", result.Error.Code);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessIC(It.IsAny<Image>()), Times.Never);
        _mockConversationsRepository.Verify(r => r.Create(It.IsAny<Conversation>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ICProcessingFails_ReturnsFailureResult()
    {
        // Arrange
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(true);

        _mockVqaServiceProvider
            .Setup(s => s.ProcessIC(It.IsAny<Image>()))
            .ReturnsAsync(Result.Failure<string>(new Error("VQA.IC_ProcessingError", "Failed to process IC.", ErrorType.Failure)));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.IC_ProcessingError", result.Error.Code);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessIC(It.IsAny<Image>()), Times.Once);
        _mockConversationsRepository.Verify(r => r.Create(It.IsAny<Conversation>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ConversationCreationFails_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(true);

        _mockVqaServiceProvider
            .Setup(s => s.ProcessIC(It.IsAny<Image>()))
            .ReturnsAsync(Result.Success("Test Caption"));

        _mockUserContext
            .Setup(u => u.UserId)
            .Returns(userId);

        _mockConversationsRepository
            .Setup(r => r.Create(It.IsAny<Conversation>()))
            .ReturnsAsync(Result.Failure<Conversation>(new Error("VQA.CREATE_CONVERSATION_ERROR", "Failed to create conversation.", ErrorType.Failure)));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.CREATE_CONVERSATION_ERROR", result.Error.Code);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessIC(It.IsAny<Image>()), Times.Once);
        _mockUserContext.Verify(u => u.UserId, Times.Once);
        _mockConversationsRepository.Verify(r => r.Create(It.IsAny<Conversation>()), Times.Once);
    }
}