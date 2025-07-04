using IdentityService.Domain.Interfaces;
using SharedKernel.Base;
using SharedKernel.Messaging;

namespace IdentityService.Application.UserProfile;

public sealed class GetUserProfileQueryHandler(IUserContext userContext, IUserManager userManager)
    : IQueryHandler<GetUserProfileQuery, GetUserProfileQueryResponse>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IUserContext _userContext = userContext;
    public async Task<Result<GetUserProfileQueryResponse>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
    {
        Guid userId;
        try
        {
            userId = _userContext.UserId;
        }
        catch (Exception)
        {
            return Result.Failure<GetUserProfileQueryResponse>(Error.Failure("USER.Unable_To_Get", "Unable to get user"));
        }
        var getUserResult = await _userManager.FindById(userId);
        if (getUserResult.IsFailure)
        {
            return Result.Failure<GetUserProfileQueryResponse>(getUserResult.Error);
        }
        var user = getUserResult.Value;
        List<String> roles = [];
        foreach (var role in user.Roles) roles.Add(role.Name);

        GetUserProfileQueryResponse response = new() { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Roles = roles };
        return response;
    }
}
