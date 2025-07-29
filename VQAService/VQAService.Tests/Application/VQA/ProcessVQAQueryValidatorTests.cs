using FluentValidation.TestHelper;
using VQAService.Application.VQA;
using VQAService.Domain.Entities.Input.Options.Language;
using VQAService.Domain.Entities.Input.Options.Model;

namespace VQAService.Tests.Application.VQA;

public class ProcessVQAQueryValidatorTests
{
    private readonly ProcessVQAQueryValidator _validator;

    public ProcessVQAQueryValidatorTests()
    {
        _validator = new ProcessVQAQueryValidator();
    }

    [Fact]
    public void Should_Have_Error_When_ConversationId_Is_Empty()
    {
        var query = new ProcessVQAQuery(
            ConversationId: Guid.Empty,
            Question: "What is in the image?",
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.ConversationId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ConversationId_Is_Valid()
    {
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.ConversationId);
    }

    [Fact]
    public void Should_Have_Error_When_Question_Is_Empty()
    {
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "",
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Question);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Question_Is_Valid()
    {
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.Question);
    }

    [Fact]
    public void Should_Have_Error_When_LanguageCode_Is_Not_Supported()
    {
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
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
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
            LanguageCode: VQALanguages.ENGLISH.Code,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.LanguageCode);
    }

    [Fact]
    public void Should_Not_Have_Error_When_LanguageCode_Is_Null()
    {
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.LanguageCode);
    }

    [Fact]
    public void Should_Have_Error_When_Model_Is_Not_Supported()
    {
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
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
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
            LanguageCode: null,
            Model: VQAModels.VLM
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.Model);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Null()
    {
        var query = new ProcessVQAQuery(
            ConversationId: Guid.NewGuid(),
            Question: "What is in the image?",
            LanguageCode: null,
            Model: null
        );

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(q => q.Model);
    }
}