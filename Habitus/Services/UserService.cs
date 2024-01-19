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
    Task<Response<IdentityResult?>> Register(RegisterRequest model);
    Response<IEnumerable<HabitusUser>> GetAll();
    Task<Response<HabitusUser>> GetById(string Id);
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
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null) return new Response<AuthenticateResponse>("Username not found");

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);

        if (isPasswordCorrect == false) new Response<AuthenticateResponse>("Invalid password");
        var token = _jwtUtils.GenerateJwtToken(user);

        output = _mapper.Map<HabitusUser, AuthenticateResponse>(user);
        output.Token = token;

        return new Response<AuthenticateResponse>(output);
    }

    public async Task<Response<IdentityResult>> Register(RegisterRequest request)
    {      
        var user = new HabitusUser
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Username,
            PhoneNumber = request.PhoneNumber,
        };

        UserValidator<HabitusUser> validator = new UserValidator<HabitusUser>();
        var validationResult = await validator.ValidateAsync(_userManager, user);

        if (validationResult.Succeeded == true)
        {
            var createUserResult = await _userManager.CreateAsync(user, request.Password);
            return new Response<IdentityResult>(createUserResult);
        }

        return new Response<IdentityResult>(validationResult);
    }

    public Response<IEnumerable<HabitusUser>> GetAll()
    {
        return new Response<IEnumerable<HabitusUser>>(_userManager.Users);
    }

    public async Task<Response<HabitusUser>> GetById(string Id)
    {
        return new Response<HabitusUser>(await _userManager.FindByIdAsync(Id));
    }
}
