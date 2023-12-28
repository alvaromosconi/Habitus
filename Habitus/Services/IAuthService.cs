using Habitus.Models.Auth;

namespace Habitus.Services;

public interface IAuthService
{
    Task<(int, string)> Registeration(Registration model, string role);
    Task<(int, string)> Login(Login model);
}