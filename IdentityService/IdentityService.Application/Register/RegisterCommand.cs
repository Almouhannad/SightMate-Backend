using SharedKernel.Messaging;

namespace IdentityService.Application.Register;

public sealed record RegisterCommand(String FirstName, String LastName, String Email, String Password) : ICommand;
