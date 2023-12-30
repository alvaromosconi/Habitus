using Habitus.Domain.Models.Auth;

namespace Habitus.Domain.Services;

public interface IAuthService
{
    Task<(int, string)> Registration(Registration model, string role);
    Task<(int, string)> Login(Login model);
    Task<HabitusUser?> GetUserByIdAsync(string userId);
}