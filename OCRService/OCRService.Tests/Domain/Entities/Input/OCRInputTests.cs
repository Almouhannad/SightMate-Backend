using Xunit;
using OCRService.Domain.Entities.Input;
using OCRService.Domain.Entities.Input.Options;
using SharedKernel.Multimedia;
using SharedKernel.Base;

namespace OCRService.Tests.Domain.Entities.Input
{
    public class OCRInputTests
    {
        [Fact]
        public void Create_ValidImageAndOptions_ReturnsSuccessResult()
        {
            // Arrange
            var image = new Image { Bytes = new List<int>([1, 2, 3]), Metadata = new Dictionary<string, object>() };
            var options = new OCROptions();

            // Act
            var result = OCRInput.Create(image, options);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(image, result.Value.Image);
            Assert.Equal(options, result.Value.Options);
        }

        [Fact]
        public void Create_ValidImageAndNullOptions_ReturnsSuccessResultWithDefaultOptions()
        {
            // Arrange
            var image = new Image { Bytes = new List<int>([1, 2, 3]), Metadata = new Dictionary<string, object>() };

            // Act
            var result = OCRInput.Create(image, null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(image, result.Value.Image);
            Assert.NotNull(result.Value.Options); // Should have default options
        }

        [Fact]
        public void Create_NullImage_ReturnsValidationFailureResult()
        {
            // Arrange
            Image image = null;
            var options = new OCROptions();

            // Act
            var result = OCRInput.Create(image, options);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Error.NullValue, result.Error);
        }
    }
}