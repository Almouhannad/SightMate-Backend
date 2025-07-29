using FluentValidation.TestHelper;
using IdentityService.Application.Register;

namespace IdentityService.Tests.Application.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var command = new RegisterCommand("", "abs", "abs@example.com", "Password123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Too_Short()
    {
        var command = new RegisterCommand("M", "abs", "abs@example.com", "Password123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        var command = new RegisterCommand("abs", "", "abs@example.com", "Password123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Too_Short()
    {
        var command = new RegisterCommand("abs", "M", "abs@example.com", "Password123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new RegisterCommand("abs", "abs", "", "Password123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid_Format()
    {
        var command = new RegisterCommand("abs", "abs", "my_awesome_email", "Password123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var command = new RegisterCommand("abs", "abs", "abs@example.com", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Password)
              .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var command = new RegisterCommand("abs", "abs", "abs@example.com", "1");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Password)
              .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Uppercase()
    {
        var command = new RegisterCommand("John", "Doe", "john.doe@example.com", "password123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Password)
              .WithErrorMessage("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Lowercase()
    {
        var command = new RegisterCommand("John", "Doe", "john.doe@example.com", "PASSWORD123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Password)
              .WithErrorMessage("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Digit()
    {
        var command = new RegisterCommand("John", "Doe", "john.doe@example.com", "Password!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Password)
              .WithErrorMessage("Password must contain at least one digit.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_SpecialChar()
    {
        var command = new RegisterCommand("John", "Doe", "john.doe@example.com", "Password123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Password)
              .WithErrorMessage("Password must contain at least one special character.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_RegisterCommand_Is_Valid()
    {
        var command = new RegisterCommand("John", "Doe", "john.doe@example.com", "Password123!");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}