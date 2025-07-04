using IdentityService.Domain.Errors;
using SharedKernel.Base;
using System.Collections.ObjectModel;
using SystemRoles = IdentityService.Domain.Entities.Roles;

namespace IdentityService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required String FirstName { get; set; }
    public required String LastName { get; set; }
    public required String Email { get; set; }
    public required String HashedPassword { get; set; }
    private readonly List<Role> _roles = [];
    public ReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public Result AddRole(String roleName)
    {
        if (SystemRoles.ROLES.TryGetValue(roleName, out var role))
        {
            _roles.Add(role!);
            return Result.Success();
        }
        return Result.Failure(UserErrors.InvalidRole);

    }
}
