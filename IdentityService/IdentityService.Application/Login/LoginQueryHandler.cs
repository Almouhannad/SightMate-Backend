using IdentityService.Domain.Errors;
using IdentityService.Domain.Interfaces;
using SharedKernel.Base;
using SharedKernel.Messaging;

namespace IdentityService.Application.Login;

public class LoginQueryHandler (IUserManager userManager, IJWTProvider jwtProvider)
    : IQueryHandler<LoginQuery, LoginQueryResponse>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IJWTProvider _jwtProvider = jwtProvider;

    async Task<Result<LoginQueryResponse>> IQueryHandler<LoginQuery, LoginQueryResponse>.Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var checkPasswordResult = await _userManager.CheckPassword(query.Email, query.Password);
        if (checkPasswordResult.IsFailure)
        {
            return Result.Failure<LoginQueryResponse> (checkPasswordResult.Error);
        }
        if (!checkPasswordResult.Value)
        {
            return Result.Failure<LoginQueryResponse>(UserErrors.NotFoundByEmail);
        }
        var getUserResult = await _userManager.FindByEmail(query.Email);
        if (getUserResult.IsFailure)
        {
            return Result.Failure<LoginQueryResponse>(getUserResult.Error);

        }
        var token = _jwtProvider.Create(getUserResult.Value);
        LoginQueryResponse response = new () { Token = token };
        return response;
    }
}
