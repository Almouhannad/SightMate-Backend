using SharedKernel.Messaging;

namespace IdentityService.Application.UserProfile;

public sealed record GetUserProfileQuery() : IQuery<GetUserProfileQueryResponse>;