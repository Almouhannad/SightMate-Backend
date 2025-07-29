using Moq;
using SharedKernel.Base;
using IdentityService.Application.Login;
using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Errors;

namespace IdentityService.Tests.Application.Login;

public class LoginQueryHandlerTests
{
    private readonly Mock<IUserManager> _mockUserManager;
    private readonly Mock<IJWTProvider> _mockJwtProvider;
    private readonly LoginQueryHandler _handler;

    public LoginQueryHandlerTests()
    {
        _mockUserManager = new Mock<IUserManager>();
        _mockJwtProvider = new Mock<IJWTProvider>();
        _handler = new LoginQueryHandler(_mockUserManager.Object, _mockJwtProvider.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "Password123!");
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com",FirstName = "Rawad", LastName = "Messi", HashedPassword= "password_hash"};

        _mockUserManager.Setup(um => um.CheckPassword(query.Email, query.Password))
            .ReturnsAsync(Result.Success(true));
        _mockUserManager.Setup(um => um.FindByEmail(query.Email))
            .ReturnsAsync(Result.Success(user));
        _mockJwtProvider.Setup(jp => jp.Create(user))
            .Returns("test_token");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("test_token", result.Value.Token);
        _mockUserManager.Verify(um => um.CheckPassword(query.Email, query.Password), Times.Once);
        _mockUserManager.Verify(um => um.FindByEmail(query.Email), Times.Once);
        _mockJwtProvider.Verify(jp => jp.Create(user), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "WrongPassword");

        _mockUserManager.Setup(um => um.CheckPassword(query.Email, query.Password))
            .ReturnsAsync(Result.Success(false));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFoundByEmail.Code, result.Error.Code);
        _mockUserManager.Verify(um => um.CheckPassword(query.Email, query.Password), Times.Once);
        _mockUserManager.Verify(um => um.FindByEmail(It.IsAny<string>()), Times.Never);
        _mockJwtProvider.Verify(jp => jp.Create(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CheckPasswordFails_ReturnsFailureResult()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "Password123!");

        _mockUserManager.Setup(um => um.CheckPassword(query.Email, query.Password))
            .ReturnsAsync(Result.Failure<bool>(UserErrors.NotFoundByEmail)); // Example error

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFoundByEmail.Code, result.Error.Code);
        _mockUserManager.Verify(um => um.CheckPassword(query.Email, query.Password), Times.Once);
        _mockUserManager.Verify(um => um.FindByEmail(It.IsAny<string>()), Times.Never);
        _mockJwtProvider.Verify(jp => jp.Create(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotFoundAfterPasswordCheck_ReturnsFailureResult()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "Password123!");

        _mockUserManager.Setup(um => um.CheckPassword(query.Email, query.Password))
            .ReturnsAsync(Result.Success(true));
        _mockUserManager.Setup(um => um.FindByEmail(query.Email))
            .ReturnsAsync(Result.Failure<User>(UserErrors.NotFoundByEmail));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFoundByEmail.Code, result.Error.Code);
        _mockUserManager.Verify(um => um.CheckPassword(query.Email, query.Password), Times.Once);
        _mockUserManager.Verify(um => um.FindByEmail(query.Email), Times.Once);
        _mockJwtProvider.Verify(jp => jp.Create(It.IsAny<User>()), Times.Never);
    }
}