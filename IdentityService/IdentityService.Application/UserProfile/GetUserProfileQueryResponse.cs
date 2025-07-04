namespace IdentityService.Application.UserProfile;

public sealed class GetUserProfileQueryResponse
{
    public required String FirstName { get; set; }
    public required String LastName { get; set; }
    public required String Email { get; set; }
    public required List<String> Roles { get; set; }
}
