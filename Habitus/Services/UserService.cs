using AutoMapper;
using Habitus.Authorization;
using Habitus.Domain.Models.Auth;
using Habitus.Domain.Services.Communication;
using Habitus.Requests;
using Habitus.Resources;
using Microsoft.AspNetCore.Identity;

namespace Habitus.Services;

public interface IUserService
{
    Task<Response<AuthenticateResponse?>> Authenticate(AuthenticateRequest model);
    Task<Response<RegisterResource>> Register(RegisterRequest model);
    Response<IEnumerable<HabitusUser>> GetAll();
    Task<Response<HabitusUser>> GetById(string Id);
    Task<Response<HabitusUser>> UpdateTelegramChatId(HabitusUser habitusUser, long chatId);
}

public class UserService : IUserService
{
    private readonly UserManager<HabitusUser> _userManager;
    private readonly IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    public UserService(UserManager<HabitusUser> userManager,
                       IMapper mapper,
                       IJwtUtils jwtUtils)
    {
        _userManager = userManager;
        _mapper = mapper;
        _jwtUtils = jwtUtils;
    }

    public async Task<Response<AuthenticateResponse?>> Authenticate(AuthenticateRequest request)
    {
        AuthenticateResponse output;
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) return new Response<AuthenticateResponse>("Email not found");

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);

        if (isPasswordCorrect == false) new Response<AuthenticateResponse>("Invalid password");
        var token = _jwtUtils.GenerateJwtToken(user);

        output = _mapper.Map<HabitusUser, AuthenticateResponse>(user);
        output.Token = token;

        return new Response<AuthenticateResponse>(output);
    }

    public async Task<Response<RegisterResource>> Register(RegisterRequest request)
    {
        var user = new HabitusUser
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
        };

        var output = _mapper.Map<HabitusUser, RegisterResource>(user);
        var validationResult = await ValidateUserAsync(user);

        string errorMessage = null;

        if (!validationResult.Succeeded)
        {
            errorMessage = validationResult.Errors.ToString();
        }
        else
        {
            var createUserResult = await _userManager.CreateAsync(user, request.Password);

            if (!createUserResult.Succeeded)
            {
                errorMessage = createUserResult.ToString();
            }
        }

        return errorMessage != null
            ? new Response<RegisterResource>(errorMessage)
            : new Response<RegisterResource>(output);
    }

    private async Task<IdentityResult> ValidateUserAsync(HabitusUser user)
    {
        var validator = new UserValidator<HabitusUser>();
        return await validator.ValidateAsync(_userManager, user);
    }

    public Response<IEnumerable<HabitusUser>> GetAll()
    {
        return new Response<IEnumerable<HabitusUser>>(_userManager.Users);
    }

    public async Task<Response<HabitusUser>> GetById(string Id)
    {
        return new Response<HabitusUser>(await _userManager.FindByIdAsync(Id));
    }

    public async Task<Response<HabitusUser>> UpdateTelegramChatId(HabitusUser habitusUser, long chatId)
    {
        habitusUser.ChatId = chatId;
        var result = await _userManager.UpdateAsync(habitusUser);

        if (result.Succeeded == true)
        {
            return new Response<HabitusUser>(habitusUser);
        }

        return new Response<HabitusUser>("Error updating telegram chat id.");
    }
}