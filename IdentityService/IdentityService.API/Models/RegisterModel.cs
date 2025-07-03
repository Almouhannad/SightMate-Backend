using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.Models;
public class RegisterModel
{
    [Required]
    public String Email { get; set; } = String.Empty;
    [Required]
    public required String Password { get; set; } = String.Empty;

}
