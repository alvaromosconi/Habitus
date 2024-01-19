﻿using AutoMapper;
using Habitus.Domain.Models.Auth;
using Habitus.Requests;
using Habitus.Resources;
using Habitus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Habitus.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]

public class UsersController : ControllerBase
{
    private IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService,
                           IMapper mapper)
    {
        _userService = userService;
    }

    /// <summary>
    /// Login
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AuthenticateResponse), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [Route("login")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
    {
        var result = await _userService.Authenticate(request);

        return Ok(result);
    }

    /// <summary>
    /// Register
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _userService.Register(request);

        if (response.Resource.Succeeded == false)
        {
            return BadRequest(new { message = response.Resource.Errors });
        }

        return Ok(response);
    }

    /// <summary>
    /// Get All Users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HabitusUser[]), 200)]
    [Authorize]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

}