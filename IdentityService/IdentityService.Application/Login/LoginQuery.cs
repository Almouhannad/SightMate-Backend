using SharedKernel.Messaging;

namespace IdentityService.Application.Login;

public sealed record LoginQuery (String Email, String Password) : IQuery<LoginQueryResponse>;
