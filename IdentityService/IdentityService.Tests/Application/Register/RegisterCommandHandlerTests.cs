using Moq;
using SharedKernel.Base;
using IdentityService.Application.Register;
using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Errors;

namespace IdentityService.Tests.Application.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserManager> _mockUserManager;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _mockUserManager = new Mock<IUserManager>();
        _handler = new RegisterCommandHandler(_mockUserManager.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new RegisterCommand("Aram", "Messi", "aram@talabaty.net", "Password123!");
        var newUser = new User { Id= Guid.NewGuid(), Email =command.Email, FirstName= $"{command.FirstName}", LastName = $"{command.LastName}", HashedPassword= "hashed_password"};

        _mockUserManager.Setup(um => um.FindByEmail(command.Email))
            .ReturnsAsync(Result.Failure<User>(UserErrors.NotFoundByEmail)); // For first check if user exists
        _mockUserManager.Setup(um => um.Create(command.FirstName, command.LastName, command.Email, command.Password))
            .ReturnsAsync(Result.Success(newUser));

        _mockUserManager.Setup(um => um.AddRole(command.Email, Roles.USER))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUserManager.Verify(um => um.Create(command.FirstName, command.LastName, command.Email, command.Password), Times.Once);
        _mockUserManager.Verify(um => um.AddRole(command.Email, Roles.USER), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateUserFails_ReturnsFailureResult()
    {
        // Arrange
        var command = new RegisterCommand("Baron", "Messi", "baron@talabaty.net", "Password123!");
        _mockUserManager.Setup(um => um.FindByEmail(command.Email))
            .ReturnsAsync(Result.Failure<User>(UserErrors.NotFoundByEmail)); // For first check if user exists
        _mockUserManager.Setup(um => um.Create(command.FirstName, command.LastName, command.Email, command.Password))
            .ReturnsAsync(Result.Failure<User>(UserErrors.EmailNotUnique)); // Example error

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.EmailNotUnique.Code, result.Error.Code);
        _mockUserManager.Verify(um => um.Create(command.FirstName, command.LastName, command.Email, command.Password), Times.Once);
        _mockUserManager.Verify(um => um.AddRole(It.IsAny<string>(), Roles.USER), Times.Never);
        _mockUserManager.Verify(um => um.AddRole(It.IsAny<string>(), Roles.ADMIN), Times.Never);
    }

    [Fact]
    public async Task Handle_AddRoleFails_ReturnsFailureResult()
    {
        // Arrange
        var command = new RegisterCommand("Rida", "Messi", "rida@talabaty.net", "Password123!");
        var newUser = new User { Id = Guid.NewGuid(), Email = command.Email, FirstName = $"{command.FirstName}", LastName = $"{command.LastName}", HashedPassword = "hashed_password" };

        _mockUserManager.Setup(um => um.Create(command.FirstName, command.LastName, command.Email, command.Password))
            .ReturnsAsync(Result.Success(newUser));
        _mockUserManager.Setup(um => um.FindByEmail(command.Email))
            .ReturnsAsync(Result.Failure<User>(UserErrors.NotFoundByEmail)); // For first check if user exists
        _mockUserManager.Setup(um => um.AddRole(command.Email, Roles.USER))
            .ReturnsAsync(Result.Failure(UserErrors.InvalidRole)); // Example error

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidRole.Code, result.Error.Code);
        _mockUserManager.Verify(um => um.Create(command.FirstName, command.LastName, command.Email, command.Password), Times.Once);
        _mockUserManager.Verify(um => um.AddRole(command.Email, Roles.USER), Times.Once);
    }
}