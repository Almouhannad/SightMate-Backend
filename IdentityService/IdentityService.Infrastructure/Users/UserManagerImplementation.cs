using IdentityService.Domain.Entities;
using IdentityService.Domain.Errors;
using IdentityService.Domain.Interfaces;
using IdentityService.Infrastructure.Users.DAOs;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Base;

namespace IdentityService.Infrastructure.Users;

public class UserManagerImplementation(UserManager<UserDAO> userManager, RoleManager<RoleDAO> roleManager) : IUserManager
{
    private readonly Error _unableToAddRole = Error.Failure("USERS.UnableToAddRole", "Field to add role");
    private readonly Error _unableToRegisterUser = Error.Failure("USERS.UnableToRegister", "Field to register user");

    private readonly UserManager<UserDAO> _userManager = userManager;
    private readonly RoleManager<RoleDAO> _roleManager = roleManager;

    public async Task<Result<User>> Create(string firstName, string lastName, string email, string password)
    {
        UserDAO userDAO = new() { FirstName = firstName, LastName = lastName, Email = email, UserName = email };
        var createUserResult = await _userManager.CreateAsync(userDAO, password);
        if (createUserResult.Succeeded)
        {
            return await FindByEmail(email);
        }
        var err = createUserResult.Errors.ElementAt(0);
        return Result.Failure<User>(Error.Conflict(err.Code, err.Description));
    }

    public async Task<Result<User>> FindByEmail(string email)
    {
        var userDAO = await _userManager.FindByEmailAsync(email);
        if (userDAO == null)
        {
            return Result.Failure<User>(UserErrors.NotFoundByEmail);
        }
        var roles = await _userManager.GetRolesAsync(userDAO);
        userDAO.Roles = [.. roles];
        return userDAO.ToDomain();
    }

    public async Task<Result<User>> FindById(Guid id)
    {
        var userDAO = await _userManager.FindByIdAsync(id.ToString());
        if (userDAO == null)
        {
            return Result.Failure<User>(UserErrors.NotFoundByEmail);
        }
        var roles = await _userManager.GetRolesAsync(userDAO);
        userDAO.Roles = [.. roles];
        return userDAO.ToDomain();
    }

    public async Task<Result<bool>> CheckPassword(String email, string password)
    {
        var userDAO = await _userManager.FindByEmailAsync(email);
        if (userDAO == null)
        {
            return Result.Failure<bool>(UserErrors.NotFoundByEmail);
        }
        var passwordCheck = await _userManager.CheckPasswordAsync(userDAO, password);
        return passwordCheck;
    }

    public async Task<Result> AddRole(String email, Role role)
    {
        if (!Roles.ROLES.TryGetValue(role.Name, out _))
        {
            return Result.Failure(UserErrors.InvalidRole);
        }

        // Create role if not exist
        if (!await _roleManager.RoleExistsAsync(role.Name))
        {
            RoleDAO roleDAO = new () { Name = role.Name };
            var addRoleResult = await _roleManager.CreateAsync(roleDAO);
            if (!addRoleResult.Succeeded)
            {
                return Result.Failure(_unableToAddRole);
            }
        }
        var userDAO = await _userManager.FindByEmailAsync(email);
        if (userDAO == null)
        {
            return Result.Failure(UserErrors.NotFoundByEmail);
        }
        var assignRoleResult = await _userManager.AddToRolesAsync(userDAO, [role.Name]);
        if (!assignRoleResult.Succeeded)
        {
            return Result.Failure(_unableToAddRole);
        }
        return Result.Success();
    }
}
