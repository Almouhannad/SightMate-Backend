using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Infrastructure.Users.DAOs;

public class UserDAO : IdentityUser<Guid>
{
    public String FirstName { get; set; } = null!;
    public String LastName { get; set; } = null!;

    [NotMapped]
    public List<String> Roles { get; set; } = [];

    public Result<User> ToDomain()
    {
        User user;
        try
        {
            user = new() {Id = Id, FirstName = FirstName, LastName = LastName, Email = Email!, HashedPassword = PasswordHash! };
        }
        catch (Exception)
        {
            return Result.Failure<User>(Error.Failure("USERS.Unable_To_Get_Domain", "Unable to convert DAO to domain"));
        }
        foreach (var role in Roles)
        {
            var addRoleResult = user.AddRole(role);
            if (addRoleResult.IsFailure)
            {
                return Result.Failure<User>(addRoleResult.Error);
            }
        }
        return user;
    }
}
