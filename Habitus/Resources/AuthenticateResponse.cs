namespace Habitus.Resources;

public class AuthenticateResponse
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Token {  get; set; }
}
