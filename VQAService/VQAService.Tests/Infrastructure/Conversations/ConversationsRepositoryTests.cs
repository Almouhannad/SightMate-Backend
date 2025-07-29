using MongoDB.Driver;
using Moq;
using SharedKernel.Multimedia;
using VQAService.Domain.Entities.Conversations;
using VQAService.Infrastructure.Conversations;
using static VQAService.Infrastructure.Conversations.DAOs.ConversationsDAOs;

namespace VQAService.Tests.Infrastructure.Conversations
{
    public class ConversationsRepositoryTests
    {
        private readonly Mock<IMongoDatabase> _mockMongoDatabase;
        private readonly Mock<IMongoCollection<ConversationDAO>> _mockCollection;
        private readonly ConversationsRepository _repository;

        public ConversationsRepositoryTests()
        {
            _mockMongoDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<ConversationDAO>>();

            _mockMongoDatabase
                .Setup(db => db.GetCollection<ConversationDAO>(
                    It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(_mockCollection.Object);

            _repository = new ConversationsRepository(_mockMongoDatabase.Object);
        }

        [Fact]
        public async Task Create_ValidConversation_ReturnsSuccessResult()
        {
            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Image = new Image { Bytes = new List<int> { 1, 2, 3 } },
                History = new List<HistoryItem>()
            };

            _mockCollection
                .Setup(c => c.InsertOneAsync(
                    It.IsAny<ConversationDAO>(),
                    null,
                    CancellationToken.None))
                .Returns(Task.CompletedTask);

            var result = await _repository.Create(conversation);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            _mockCollection.Verify(c => c.InsertOneAsync(
                It.IsAny<ConversationDAO>(),
                null,
                CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task Create_ExceptionThrown_ReturnsFailureResult()
        {
            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Image = new Image { Bytes = new List<int> { 1, 2, 3 } },
                History = new List<HistoryItem>()
            };

            _mockCollection
                .Setup(c => c.InsertOneAsync(
                    It.IsAny<ConversationDAO>(),
                    null,
                    CancellationToken.None))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _repository.Create(conversation);

            Assert.False(result.IsSuccess);
            Assert.Equal("VQA.CREATE_CONVERSATION_ERROR", result.Error.Code);
        }

        [Fact]
        public async Task GetById_ConversationExists_ReturnsSuccessResult()
        {
            var conversationId = Guid.NewGuid();
            var conversationDAO = new ConversationDAO
            {
                Id = conversationId,
                UserId = Guid.NewGuid(),
                Image = new ImageDAO { Bytes = new List<int> { 1, 2, 3 } },
                History = new List<HistoryItemDAO>()
            };

            var mockCursor = new Mock<IAsyncCursor<ConversationDAO>>();
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor
                .SetupGet(c => c.Current)
                .Returns(new[] { conversationDAO });

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ConversationDAO>>(),
                    It.IsAny<FindOptions<ConversationDAO, ConversationDAO>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var result = await _repository.GetById(conversationId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(conversationId, result.Value.Id);
        }

        [Fact]
        public async Task GetById_ConversationDoesNotExist_ReturnsFailureResult()
        {
            var conversationId = Guid.NewGuid();

            // Cursor that never yields anything
            var emptyCursor = new Mock<IAsyncCursor<ConversationDAO>>();
            emptyCursor
                .Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            emptyCursor
                .SetupGet(c => c.Current)
                .Returns(Array.Empty<ConversationDAO>());

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ConversationDAO>>(),
                    It.IsAny<FindOptions<ConversationDAO, ConversationDAO>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyCursor.Object);

            var result = await _repository.GetById(conversationId);

            Assert.False(result.IsSuccess);
            Assert.Equal("VQA.CONVERSATION_NOT_FOUND", result.Error.Code);
        }

        [Fact]
        public async Task GetById_ExceptionThrown_ReturnsFailureResult()
        {
            var conversationId = Guid.NewGuid();

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ConversationDAO>>(),
                    It.IsAny<FindOptions<ConversationDAO, ConversationDAO>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _repository.GetById(conversationId);

            Assert.False(result.IsSuccess);
            Assert.Equal("VQA.GET_CONVERSATION_ERROR", result.Error.Code);
        }

        [Fact]
        public async Task UpdateHistory_ConversationExists_ReturnsSuccessResult()
        {
            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Image = new Image { Bytes = new List<int> { 1, 2, 3 } },
                History = new List<HistoryItem>()
            };
            var newHistory = new List<HistoryItem>
            {
                new HistoryItem { Question = "Q1", Answer = "A1" }
            };

            _mockCollection
                .Setup(c => c.UpdateOneAsync(
                    It.IsAny<FilterDefinition<ConversationDAO>>(),
                    It.IsAny<UpdateDefinition<ConversationDAO>>(),
                    It.IsAny<UpdateOptions>(),
                    CancellationToken.None))
                .ReturnsAsync(new UpdateResult.Acknowledged(1, 1, new MongoDB.Bson.BsonInt64(1)));

            var result = await _repository.UpdateHistory(conversation, newHistory);

            Assert.True(result.IsSuccess);
            _mockCollection.Verify(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<ConversationDAO>>(),
                It.IsAny<UpdateDefinition<ConversationDAO>>(),
                It.IsAny<UpdateOptions>(),
                CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task UpdateHistory_ConversationDoesNotExist_ReturnsFailureResult()
        {
            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Image = new Image { Bytes = new List<int> { 1, 2, 3 } },
                History = new List<HistoryItem>()
            };
            var newHistory = new List<HistoryItem>
            {
                new HistoryItem { Question = "Q1", Answer = "A1" }
            };

            _mockCollection
                .Setup(c => c.UpdateOneAsync(
                    It.IsAny<FilterDefinition<ConversationDAO>>(),
                    It.IsAny<UpdateDefinition<ConversationDAO>>(),
                    It.IsAny<UpdateOptions>(),
                    CancellationToken.None))
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 0, new MongoDB.Bson.BsonInt64(0)));

            var result = await _repository.UpdateHistory(conversation, newHistory);

            Assert.False(result.IsSuccess);
            Assert.Equal("VQA.CONVERSATION_NOT_FOUND", result.Error.Code);
        }

        [Fact]
        public async Task UpdateHistory_ExceptionThrown_ReturnsFailureResult()
        {
            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Image = new Image { Bytes = new List<int> { 1, 2, 3 } },
                History = new List<HistoryItem>()
            };
            var newHistory = new List<HistoryItem>
            {
                new HistoryItem { Question = "Q1", Answer = "A1" }
            };

            _mockCollection
                .Setup(c => c.UpdateOneAsync(
                    It.IsAny<FilterDefinition<ConversationDAO>>(),
                    It.IsAny<UpdateDefinition<ConversationDAO>>(),
                    It.IsAny<UpdateOptions>(),
                    CancellationToken.None))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _repository.UpdateHistory(conversation, newHistory);

            Assert.False(result.IsSuccess);
            Assert.Equal("VQA.UPDATE_CONVERSATION_ERROR", result.Error.Code);
        }
    }
}
