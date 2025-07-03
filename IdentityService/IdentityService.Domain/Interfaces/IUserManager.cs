using IdentityService.Domain.Entities;
using SharedKernel.Base;

namespace IdentityService.Domain.Interfaces;

public interface IUserManager
{
    public Task<Result<User>> Create(String firstName, String lastName, String email, String password, List<Role>? roles);

    public Task<Result<User>> FindByEmail(String email);
    public Task<Result<bool>> CheckPassword(User user, String password);
    public Task<Result> AddRole(User user, Role role);


}
