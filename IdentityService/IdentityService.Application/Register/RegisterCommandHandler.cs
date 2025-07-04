using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using SharedKernel.Base;
using SharedKernel.Messaging;

namespace IdentityService.Application.Register;

public sealed class RegisterCommandHandler (IUserManager userManager)
    : ICommandHandler<RegisterCommand>
{
    private readonly IUserManager _userManager = userManager;
    public async Task<Result> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var createUserReault = await _userManager.Create(command.FirstName, command.LastName, command.Email, command.Password);
        if (createUserReault.IsFailure)
        {
            return Result.Failure(createUserReault.Error);
        }
        var addUserRoleResult = await _userManager.AddRole(createUserReault.Value.Email, Roles.USER);
        if (addUserRoleResult.IsFailure)
        {
            return Result.Failure(addUserRoleResult.Error);
        }
        return Result.Success();
    }
}
