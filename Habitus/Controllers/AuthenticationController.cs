using Habitus.Models.Auth;
using Habitus.Services;
using Microsoft.AspNetCore.Mvc;

namespace Habitus.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthService authService, ILogger<AuthenticationController> logger)
    {
        _authService = authService;
        _logger = logger;
    }


    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(Login model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");
            var (status, message) = await _authService.Login(model);
            if (status == 0)
                return BadRequest(message);
            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(Registration model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");
            var (status, message) = await _authService.Registeration(model, UserRoles.User);
            if (status == 0)
            {
                return BadRequest(message);
            }
            return CreatedAtAction(nameof(Register), model);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}