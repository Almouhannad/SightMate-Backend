using IdentityService.Domain.Entities;
using SharedKernel.Base;

namespace IdentityService.Domain.Interfaces;

public interface IJWTProvider
{
    public Result<String> Create(User user);
}