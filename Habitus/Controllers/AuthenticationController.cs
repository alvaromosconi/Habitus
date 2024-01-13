using Habitus.Domain.Models.Auth;
using Habitus.Domain.Services;
using Habitus.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    /// <summary>
    /// Log in
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [Route("login")]
    public async Task<IActionResult> Login(Login model)
    {
        try
        {   
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

    /// <summary>
    /// Register
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(HabitusUserResource), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [Route("register")]
    public async Task<IActionResult> Register(Registration model)
    {
        try
        { 
            var (status, message) = await _authService.Registration(model, UserRoles.User);
            
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

    /// <summary>
    /// Get user profile
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(HabitusUserResource), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    [Route("user")]
    public async Task<IActionResult> UserProfile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

}