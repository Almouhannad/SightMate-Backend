using SharedKernel.Base;

namespace IdentityService.Domain.Errors;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "Incorrect email or password");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");

    public static readonly Error InvalidRole = Error.Conflict(
        "Users.InvalidRole",
        "Selected role is invalid");
}
