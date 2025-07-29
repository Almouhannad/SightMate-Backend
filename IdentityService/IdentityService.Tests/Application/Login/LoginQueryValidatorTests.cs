using FluentValidation.TestHelper;
using IdentityService.Application.Login;

namespace IdentityService.Tests.Application.Login;

public class LoginQueryValidatorTests
{
    private readonly LoginQueryValidator _validator;

    public LoginQueryValidatorTests()
    {
        _validator = new LoginQueryValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var query = new LoginQuery("", "Password123!");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid_Format()
    {
        var query = new LoginQuery("invalid-email", "Password123!");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var query = new LoginQuery("test@example.com", "");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Password)
              .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var query = new LoginQuery("test@example.com", "Short1!");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Password)
              .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Uppercase()
    {
        var query = new LoginQuery("test@example.com", "password123!");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Password)
              .WithErrorMessage("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Lowercase()
    {
        var query = new LoginQuery("test@example.com", "PASSWORD123!");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Password)
              .WithErrorMessage("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Digit()
    {
        var query = new LoginQuery("test@example.com", "Password!!!");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Password)
              .WithErrorMessage("Password must contain at least one digit.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_SpecialChar()
    {
        var query = new LoginQuery("test@example.com", "Password123");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Password)
              .WithErrorMessage("Password must contain at least one special character.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_LoginQuery_Is_Valid()
    {
        var query = new LoginQuery("test@example.com", "Password123!");
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}