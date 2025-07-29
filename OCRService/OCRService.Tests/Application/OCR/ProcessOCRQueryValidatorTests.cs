using FluentValidation.TestHelper;
using OCRService.Application.OCR;
using OCRService.Domain.Entities.Input.Options.Model;

namespace OCRService.Tests.Application.OCR
{
    public class ProcessOCRQueryValidatorTests
    {
        private readonly ProcessOCRQueryValidator _validator;

        public ProcessOCRQueryValidatorTests()
        {
            _validator = new ProcessOCRQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_ImageBytes_Is_Empty()
        {
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: null,
                Model: null
            );

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.ImageBytes);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ImageBytes_Is_Valid()
        {
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1, 2, 3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: null,
                Model: null
            );

            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.ImageBytes);
        }

        [Fact]
        public void Should_Have_Error_When_ImageBytes_Contains_Invalid_Value()
        {
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1, 2, 256]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: null,
                Model: null
            );

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.ImageBytes)
                  .WithErrorCode("ImageInputValuesValidator");
        }

        [Fact]
        public void Should_Have_Error_When_LanguageCode_Is_Not_Supported()
        {
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1,2,3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: "messi",
                Model: null
            );

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.LanguageCode)
                  .WithErrorCode("NotSupportedLanguageValidator");
        }

        [Fact]
        public void Should_Not_Have_Error_When_LanguageCode_Is_Supported()
        {
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1, 2, 3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: "en",
                Model: null
            );

            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.LanguageCode);
        }

        [Fact]
        public void Should_Not_Have_Error_When_LanguageCode_Is_Null()
        {
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1, 2, 3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: null,
                Model: null
            );

            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.LanguageCode);
        }

        [Fact]
        public void Should_Have_Error_When_Model_Is_Not_Supported()
        {
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1, 2, 3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: null,
                Model: "messi"
            );

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.Model)
                  .WithErrorCode("NotSupportedModelValidator");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Supported()
        {
            // Assuming "default" is a supported model based on OCRModels.OCRSupportedModelsMap
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1, 2, 3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: null,
                Model: "easyocr"
            );

            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.Model);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Null()
        {
            var query = new ProcessOCRQuery(
                ImageBytes: new List<int>([1,2, 3]),
                ImageMetadata: new Dictionary<string, object>(),
                Describe: false,
                LanguageCode: null,
                Model: null
            );

            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.Model);
        }
    }
}