using Moq;
using SharedKernel.Base;
using SharedKernel.Multimedia;
using VQAService.Application.VQA;
using VQAService.Domain.Entities.Conversations;
using VQAService.Domain.Interfaces;
using IdentityService.Domain.Interfaces;

namespace VQAService.Tests.Application.VQA;

public class ProcessVQAQueryHandlerTests
{
    private readonly Mock<IVQAServiceProvider> _mockVqaServiceProvider;
    private readonly Mock<IConversationsRepository> _mockConversationsRepository;
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly ProcessVQAQueryHandler _handler;

    public ProcessVQAQueryHandlerTests()
    {
        _mockVqaServiceProvider = new Mock<IVQAServiceProvider>();
        _mockConversationsRepository = new Mock<IConversationsRepository>();
        _mockUserContext = new Mock<IUserContext>();
        _handler = new ProcessVQAQueryHandler(_mockVqaServiceProvider.Object, _mockConversationsRepository.Object, _mockUserContext.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResultAndUpdatesConversationHistory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var existingConversation = new Conversation
        {
            Id = conversationId,
            UserId = userId,
            Image = new Image { Bytes = new List<int> { 10, 20, 30 } },
            History = new List<HistoryItem>()
        };

        var query = new ProcessVQAQuery(
            ConversationId: conversationId,
            Question: "What is in the image?",
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(true);

        _mockConversationsRepository
            .Setup(r => r.GetById(conversationId))
            .ReturnsAsync(Result.Success(existingConversation));

        _mockUserContext
            .Setup(u => u.UserId)
            .Returns(userId);

        _mockVqaServiceProvider
            .Setup(s => s.ProcessVQA(It.IsAny<Image>(), It.IsAny<string>(), It.IsAny<List<HistoryItem>>()))
            .ReturnsAsync(Result.Success("The answer is here."));

        _mockConversationsRepository
            .Setup(r => r.UpdateHistory(It.IsAny<Conversation>(), It.IsAny<List<HistoryItem>>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("The answer is here.", result.Value.Answer);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockConversationsRepository.Verify(r => r.GetById(conversationId), Times.Once);
        _mockUserContext.Verify(u => u.UserId, Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessVQA(existingConversation.Image, query.Question, existingConversation.History), Times.Once);
        _mockConversationsRepository.Verify(r => r.UpdateHistory(existingConversation, It.Is<List<HistoryItem>>(h => h.Count == 1 && h[0].Question == query.Question && h[0].Answer == "The answer is here.")), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceNotAvailable_ReturnsFailureResult()
    {
        // Arrange
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
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
        _mockConversationsRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        _mockVqaServiceProvider.Verify(s => s.ProcessVQA(It.IsAny<Image>(), It.IsAny<string>(), It.IsAny<List<HistoryItem>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ConversationNotFound_ReturnsFailureResult()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var query = new ProcessVQAQuery(
            ConversationId: conversationId,
            Question: "What is in the image?",
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(true);

        _mockConversationsRepository
            .Setup(r => r.GetById(conversationId))
            .ReturnsAsync(Result.Failure<Conversation>(new Error("VQA.CONVERSATION_NOT_FOUND", "Conversation not found", ErrorType.NotFound)));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.CONVERSATION_NOT_FOUND", result.Error.Code);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockConversationsRepository.Verify(r => r.GetById(conversationId), Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessVQA(It.IsAny<Image>(), It.IsAny<string>(), It.IsAny<List<HistoryItem>>()), Times.Never);
        _mockConversationsRepository.Verify(r => r.UpdateHistory(It.IsAny<Conversation>(), It.IsAny<List<HistoryItem>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UnauthorizedConversationAccess_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var existingConversation = new Conversation
        {
            Id = conversationId,
            UserId = anotherUserId, // Different user ID
            Image = new Image { Bytes = new List<int> { 10, 20, 30 } },
            History = new List<HistoryItem>()
        };

        var query = new ProcessVQAQuery(
            ConversationId: conversationId,
            Question: "What is in the image?",
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(true);

        _mockConversationsRepository
            .Setup(r => r.GetById(conversationId))
            .ReturnsAsync(Result.Success(existingConversation));

        _mockUserContext
            .Setup(u => u.UserId)
            .Returns(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.Forbidden", result.Error.Code);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockConversationsRepository.Verify(r => r.GetById(conversationId), Times.Once);
        _mockUserContext.Verify(u => u.UserId, Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessVQA(It.IsAny<Image>(), It.IsAny<string>(), It.IsAny<List<HistoryItem>>()), Times.Never);
        _mockConversationsRepository.Verify(r => r.UpdateHistory(It.IsAny<Conversation>(), It.IsAny<List<HistoryItem>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_VQAProcessingFails_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var existingConversation = new Conversation
        {
            Id = conversationId,
            UserId = userId,
            Image = new Image { Bytes = new List<int> { 10, 20, 30 } },
            History = new List<HistoryItem>()
        };

        var query = new ProcessVQAQuery(
            ConversationId: conversationId,
            Question: "What is in the image?",
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(true);

        _mockConversationsRepository
            .Setup(r => r.GetById(conversationId))
            .ReturnsAsync(Result.Success(existingConversation));

        _mockUserContext
            .Setup(u => u.UserId)
            .Returns(userId);

        _mockVqaServiceProvider
            .Setup(s => s.ProcessVQA(It.IsAny<Image>(), It.IsAny<string>(), It.IsAny<List<HistoryItem>>()))
            .ReturnsAsync(Result.Failure<string>(new Error("VQA.ProcessingError", "Failed to process VQA.", ErrorType.Failure)));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.ProcessingError", result.Error.Code);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockConversationsRepository.Verify(r => r.GetById(conversationId), Times.Once);
        _mockUserContext.Verify(u => u.UserId, Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessVQA(existingConversation.Image, query.Question, existingConversation.History), Times.Once);
        _mockConversationsRepository.Verify(r => r.UpdateHistory(It.IsAny<Conversation>(), It.IsAny<List<HistoryItem>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateHistoryFails_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var existingConversation = new Conversation
        {
            Id = conversationId,
            UserId = userId,
            Image = new Image { Bytes = new List<int> { 10, 20, 30 } },
            History = new List<HistoryItem>()
        };

        var query = new ProcessVQAQuery(
            ConversationId: conversationId,
            Question: "What is in the image?",
            LanguageCode: "en",
            Model: "vlm"
        );

        _mockVqaServiceProvider
            .Setup(s => s.IsAvailable())
            .ReturnsAsync(true);

        _mockConversationsRepository
            .Setup(r => r.GetById(conversationId))
            .ReturnsAsync(Result.Success(existingConversation));

        _mockUserContext
            .Setup(u => u.UserId)
            .Returns(userId);

        _mockVqaServiceProvider
            .Setup(s => s.ProcessVQA(It.IsAny<Image>(), It.IsAny<string>(), It.IsAny<List<HistoryItem>>()))
            .ReturnsAsync(Result.Success("The answer is here."));

        _mockConversationsRepository
            .Setup(r => r.UpdateHistory(It.IsAny<Conversation>(), It.IsAny<List<HistoryItem>>()))
            .ReturnsAsync(Result.Failure(new Error("VQA.UPDATE_CONVERSATION_ERROR", "Failed to update conversation history.", ErrorType.Failure)));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VQA.UPDATE_CONVERSATION_ERROR", result.Error.Code);
        _mockVqaServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
        _mockConversationsRepository.Verify(r => r.GetById(conversationId), Times.Once);
        _mockUserContext.Verify(u => u.UserId, Times.Once);
        _mockVqaServiceProvider.Verify(s => s.ProcessVQA(existingConversation.Image, query.Question, existingConversation.History), Times.Once);
        _mockConversationsRepository.Verify(r => r.UpdateHistory(existingConversation, It.IsAny<List<HistoryItem>>()), Times.Once);
    }
}