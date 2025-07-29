using Moq;
using OCRService.Application.OCR;
using OCRService.Domain.Entities.Input;
using OCRService.Domain.Entities.Output;
using OCRService.Domain.Interfaces;
using SharedKernel.Base;


namespace OCRService.Tests.Application.OCR
{
    public class ProcessOCRQueryHandlerTests
    {
        private readonly Mock<IOCRServiceProvider> _mockOcrServiceProvider;
        private readonly ProcessOCRQueryHandler _handler;

        public ProcessOCRQueryHandlerTests()
        {
            _mockOcrServiceProvider = new Mock<IOCRServiceProvider>();
            _handler = new ProcessOCRQueryHandler(_mockOcrServiceProvider.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_ReturnsSuccessResult()
        {
            // Arrange
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1,2,3]),
                ImageMetadata: new Dictionary<string, object> (),
                Describe: true,
                LanguageCode: "en",
                Model: "test_model"
            );

            _mockOcrServiceProvider
                .Setup(s => s.IsAvailable())
                .ReturnsAsync(true);

            _mockOcrServiceProvider
                .Setup(s => s.ProcessOCR(It.IsAny<OCRInput>()))
                .ReturnsAsync(Result.Success(new OCROutput()));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            _mockOcrServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
            _mockOcrServiceProvider.Verify(s => s.ProcessOCR(It.IsAny<OCRInput>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ServiceNotAvailable_ReturnsFailureResult()
        {
            // Arrange
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1, 2, 3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: true,
                LanguageCode: "en",
                Model: "test_model"
            );

            _mockOcrServiceProvider
                .Setup(s => s.IsAvailable())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("OCR.SERVICE_NOT_AVAILABLE", result.Error.Code);
            _mockOcrServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
            _mockOcrServiceProvider.Verify(s => s.ProcessOCR(It.IsAny<OCRInput>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OCRProcessingFails_ReturnsFailureResult()
        {
            // Arrange
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1, 2, 3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: true,
                LanguageCode: "en",
                Model: "test_model"
            );

            _mockOcrServiceProvider
                .Setup(s => s.IsAvailable())
                .ReturnsAsync(true);

            _mockOcrServiceProvider
                .Setup(s => s.ProcessOCR(It.IsAny<OCRInput>()))
                .ReturnsAsync(Result.Failure<OCROutput>(new Error("OCR.ProcessingError", "Failed to process OCR.", ErrorType.Failure)));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("OCR.ProcessingError", result.Error.Code);
            _mockOcrServiceProvider.Verify(s => s.IsAvailable(), Times.Once);
            _mockOcrServiceProvider.Verify(s => s.ProcessOCR(It.IsAny<OCRInput>()), Times.Once);
        }
    }
}