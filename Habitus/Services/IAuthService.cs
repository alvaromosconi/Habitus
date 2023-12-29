using Habitus.Models.Auth;

namespace Habitus.Services;

public interface IAuthService
{
    Task<(int, string)> Registration(Registration model, string role);
    Task<(int, string)> Login(Login model);
}