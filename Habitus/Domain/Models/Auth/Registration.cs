using System.ComponentModel.DataAnnotations;

namespace Habitus.Domain.Models.Auth;

public class Registration
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
}
