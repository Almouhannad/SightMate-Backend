using FluentValidation.TestHelper;
using VQAService.Application.IC;
using VQAService.Domain.Entities.Input.Options.Language;
using VQAService.Domain.Entities.Input.Options.Model;

namespace VQAService.Tests.Application.IC;

public class ProcessICQueryValidatorTests
{
    private readonly ProcessICQueryValidator _validator;

    public ProcessICQueryValidatorTests()
    {
        _validator = new ProcessICQueryValidator();
    }

    [Fact]
    public void Should_Have_Error_When_ImageBytes_Is_Empty()
    {
        var query = new ProcessICQuery(
            ImageBytes: new List<int>(),
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.ImageBytes);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ImageBytes_Is_Valid()
    {
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.ImageBytes);
    }

    [Fact]
    public void Should_Have_Error_When_ImageBytes_Contains_Invalid_Value()
    {
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 256 },
            ImageMetadata: new Dictionary<string, object>(),
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
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: "unsupported",
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.LanguageCode)
              .WithErrorCode("NotSupportedLanguageValidator");
    }

    [Fact]
    public void Should_Not_Have_Error_When_LanguageCode_Is_Supported()
    {
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: VQALanguages.ENGLISH.Code,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.LanguageCode);
    }

    [Fact]
    public void Should_Not_Have_Error_When_LanguageCode_Is_Null()
    {
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.LanguageCode);
    }

    [Fact]
    public void Should_Have_Error_When_Model_Is_Not_Supported()
    {
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: null,
            Model: "unsupported_model"
        );

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Model)
              .WithErrorCode("NotSupportedModelValidator");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Supported()
    {
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: null,
            Model: VQAModels.VLM
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.Model);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Null()
    {
        var query = new ProcessICQuery(
            ImageBytes: new List<int> { 1, 2, 3 },
            ImageMetadata: new Dictionary<string, object>(),
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.Model);
    }
}