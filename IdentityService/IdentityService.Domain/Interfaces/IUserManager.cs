using IdentityService.Domain.Entities;
using SharedKernel.Base;

namespace IdentityService.Domain.Interfaces;

public interface IUserManager
{
    public Task<Result<User>> Create(String firstName, String lastName, String email, String password);

    public Task<Result<User>> FindByEmail(String email);
    public Task<Result<bool>> CheckPassword(String email, String password);
    public Task<Result> AddRole(String email, Role role);


}
