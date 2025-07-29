using Moq;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Errors;
using IdentityService.Infrastructure.Users;
using IdentityService.Infrastructure.Users.DAOs;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.Infrastructure.Users;

public class UserManagerImplementationTests
{
    private readonly Mock<UserManager<UserDAO>> _mockUserManager;
    private readonly Mock<RoleManager<RoleDAO>> _mockRoleManager;
    private readonly UserManagerImplementation _userManagerImplementation;

    public UserManagerImplementationTests()
    {
        var userStore = new Mock<IUserStore<UserDAO>>();
        _mockUserManager = new Mock<UserManager<UserDAO>>(
            userStore.Object,
            null, null, null, null, null, null, null, null); // Unrequired services

        var roleStore = new Mock<IRoleStore<RoleDAO>>();
        _mockRoleManager = new Mock<RoleManager<RoleDAO>>(
            roleStore.Object,
            null, null, null, null); // Unrequired services

        _userManagerImplementation = new UserManagerImplementation(_mockUserManager.Object, _mockRoleManager.Object);
    }

    [Fact]
    public async Task Create_ValidUser_ReturnsSuccessWithUser()
    {
        // Arrange
        var firstName = "Test";
        var lastName = "User";
        var email = "test@example.com";
        var password = "Password123!";
        var userDAO = new UserDAO { Id = Guid.NewGuid(), Email = email, UserName = email, FirstName = firstName, LastName = lastName };

        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<UserDAO>(), password))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(userDAO);
        _mockUserManager.Setup(um => um.GetRolesAsync(userDAO))
            .ReturnsAsync(new List<string>()); // No roles initially

        // Act
        var result = await _userManagerImplementation.Create(firstName, lastName, email, password);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(email, result.Value.Email);
        _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<UserDAO>(), password), Times.Once);
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task Create_UserCreationFails_ReturnsFailureResult()
    {
        // Arrange
        var firstName = "Test";
        var lastName = "User";
        var email = "test@example.com";
        var password = "Password123!";

        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<UserDAO>(), password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email already taken." }));

        // Act
        var result = await _userManagerImplementation.Create(firstName, lastName, email, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateEmail", result.Error.Code);
        _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<UserDAO>(), password), Times.Once);
        _mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task FindByEmail_UserExists_ReturnsSuccessWithUser()
    {
        // Arrange
        var email = "test@example.com";
        var userDAO = new UserDAO { Email = email, UserName = email, Id = Guid.NewGuid() };
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(userDAO);
        _mockUserManager.Setup(um => um.GetRolesAsync(userDAO))
            .ReturnsAsync([Roles.USER.Name]);

        // Act
        var result = await _userManagerImplementation.FindByEmail(email);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(email, result.Value.Email);
        Assert.Contains(result.Value.Roles, r => r.Name == Roles.USER.Name);
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.GetRolesAsync(userDAO), Times.Once);
    }

    [Fact]
    public async Task FindByEmail_UserDoesNotExist_ReturnsFailureResult()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(() => default);

        // Act
        var result = await _userManagerImplementation.FindByEmail(email);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFoundByEmail.Code, result.Error.Code);
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.GetRolesAsync(It.IsAny<UserDAO>()), Times.Never);
    }

    [Fact]
    public async Task FindById_UserExists_ReturnsSuccessWithUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDAO = new UserDAO { Id = userId, Email = "test@example.com", UserName = "test@example.com" };
        _mockUserManager.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(userDAO);
        _mockUserManager.Setup(um => um.GetRolesAsync(userDAO))
            .ReturnsAsync([Roles.ADMIN.Name]);

        // Act
        var result = await _userManagerImplementation.FindById(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.Id);
        Assert.Contains(result.Value.Roles, r => r.Name == Roles.ADMIN.Name);
        _mockUserManager.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);
        _mockUserManager.Verify(um => um.GetRolesAsync(userDAO), Times.Once);
    }

    [Fact]
    public async Task FindById_UserDoesNotExist_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserManager.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(() => default);

        // Act
        var result = await _userManagerImplementation.FindById(userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFoundByEmail.Code, result.Error.Code); // Note: Current implementation returns NotFoundByEmail for FindById
        _mockUserManager.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);
        _mockUserManager.Verify(um => um.GetRolesAsync(It.IsAny<UserDAO>()), Times.Never);
    }

    [Fact]
    public async Task CheckPassword_CorrectPassword_ReturnsSuccessTrue()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var userDAO = new UserDAO { Email = email, UserName = email };
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(userDAO);
        _mockUserManager.Setup(um => um.CheckPasswordAsync(userDAO, password))
            .ReturnsAsync(true);

        // Act
        var result = await _userManagerImplementation.CheckPassword(email, password);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(userDAO, password), Times.Once);
    }

    [Fact]
    public async Task CheckPassword_IncorrectPassword_ReturnsSuccessFalse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword";
        var userDAO = new UserDAO { Email = email, UserName = email };
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(userDAO);
        _mockUserManager.Setup(um => um.CheckPasswordAsync(userDAO, password))
            .ReturnsAsync(false);

        // Act
        var result = await _userManagerImplementation.CheckPassword(email, password);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(userDAO, password), Times.Once);
    }

    [Fact]
    public async Task CheckPassword_UserDoesNotExist_ReturnsFailureResult()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "Password123!";
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(() => default);

        // Act
        var result = await _userManagerImplementation.CheckPassword(email, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFoundByEmail.Code, result.Error.Code);
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<UserDAO>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AddRole_ValidRoleAndUser_ReturnsSuccessResult()
    {
        // Arrange
        var email = "test@example.com";
        var role = Roles.ADMIN;
        var userDAO = new UserDAO { Email = email, UserName = email };

        _mockRoleManager.Setup(rm => rm.RoleExistsAsync(role.Name))
            .ReturnsAsync(true); // Role already exists
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(userDAO);
        _mockUserManager.Setup(um => um.AddToRolesAsync(userDAO, It.Is<IEnumerable<string>>(r => r.Contains(role.Name))))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userManagerImplementation.AddRole(email, role);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRoleManager.Verify(rm => rm.RoleExistsAsync(role.Name), Times.Once);
        _mockRoleManager.Verify(rm => rm.CreateAsync(It.IsAny<RoleDAO>()), Times.Never); // Should not create role
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.AddToRolesAsync(userDAO, It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    [Fact]
    public async Task AddRole_RoleDoesNotExist_CreatesRoleAndAddsToUser()
    {
        // Arrange
        var email = "test@example.com";
        var role = Roles.ADMIN; // Assuming ADMIN role
        var userDAO = new UserDAO { Email = email, UserName = email };

        _mockRoleManager.Setup(rm => rm.RoleExistsAsync(role.Name))
            .ReturnsAsync(false); // Role does not exist
        _mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<RoleDAO>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(userDAO);
        _mockUserManager.Setup(um => um.AddToRolesAsync(userDAO, It.Is<IEnumerable<string>>(r => r.Contains(role.Name))))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userManagerImplementation.AddRole(email, role);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRoleManager.Verify(rm => rm.RoleExistsAsync(role.Name), Times.Once);
        _mockRoleManager.Verify(rm => rm.CreateAsync(It.IsAny<RoleDAO>()), Times.Once); // Should create role
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.AddToRolesAsync(userDAO, It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    [Fact]
    public async Task AddRole_InvalidRole_ReturnsFailureResult()
    {
        // Arrange
        var email = "test@example.com";
        var invalidRole = new Role { Name = "Invalid role" }; // Create an invalid role

        // Act
        var result = await _userManagerImplementation.AddRole(email, invalidRole);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidRole.Code, result.Error.Code);
        _mockRoleManager.Verify(rm => rm.RoleExistsAsync(It.IsAny<string>()), Times.Never);
        _mockRoleManager.Verify(rm => rm.CreateAsync(It.IsAny<RoleDAO>()), Times.Never);
        _mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Never);
        _mockUserManager.Verify(um => um.AddToRolesAsync(It.IsAny<UserDAO>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task AddRole_UserDoesNotExist_ReturnsFailureResult()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var role = Roles.USER;
        _mockRoleManager.Setup(rm => rm.RoleExistsAsync(role.Name))
            .ReturnsAsync(true);
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(() => default);

        // Act
        var result = await _userManagerImplementation.AddRole(email, role);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFoundByEmail.Code, result.Error.Code);
        _mockRoleManager.Verify(rm => rm.RoleExistsAsync(role.Name), Times.Once);
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.AddToRolesAsync(It.IsAny<UserDAO>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task AddRole_AddToRolesFails_ReturnsFailureResult()
    {
        // Arrange
        var email = "test@example.com";
        var role = Roles.USER;
        var userDAO = new UserDAO { Email = email, UserName = email };

        _mockRoleManager.Setup(rm => rm.RoleExistsAsync(role.Name))
            .ReturnsAsync(true);
        _mockUserManager.Setup(um => um.FindByEmailAsync(email))
            .ReturnsAsync(userDAO);
        _mockUserManager.Setup(um => um.AddToRolesAsync(userDAO, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "FailedToAddRole", Description = "Could not add user to role." }));

        // Act
        var result = await _userManagerImplementation.AddRole(email, role);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("USERS.UnableToAddRole", result.Error.Code);
        _mockRoleManager.Verify(rm => rm.RoleExistsAsync(role.Name), Times.Once);
        _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _mockUserManager.Verify(um => um.AddToRolesAsync(userDAO, It.IsAny<IEnumerable<string>>()), Times.Once);
    }
}