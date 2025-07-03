using System.Collections.ObjectModel;

namespace IdentityService.Domain.Entities;

public static class Roles
{
    private static readonly Role _user = new() { Name = "USER" };
    public static Role USER => _user;

    private static readonly Role _admin = new() { Name = "ADMIN" };
    public static Role ADMIN => _admin;

    public static readonly ReadOnlyDictionary<string, Role> ROLES =
        new(
            new Dictionary<string, Role>(StringComparer.OrdinalIgnoreCase)
            {
                    { _user.Name, USER },
                    { _admin.Name, ADMIN }
            }
        );
}
