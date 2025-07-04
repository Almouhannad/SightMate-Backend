using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Base;

namespace IdentityService.Infrastructure.Users.DAOs;
public class RoleDAO : IdentityRole<Guid>
{

    public Result<Role> ToDomain()
    {
        Role role;
		try
		{
            role = new() { Name = Name! };
            return role;
        }
        catch (Exception)
		{
            return Result.Failure<Role>(Error.Failure("USERS.Unable_To_Get_Domain", "Unable to convert DAO to domain"));
        }
    }
}
