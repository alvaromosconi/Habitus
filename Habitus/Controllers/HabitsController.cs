using AutoMapper;
using Elfie.Serialization;
using Habitus.Domain.Models;
using Habitus.Domain.Models.Auth;
using Habitus.Domain.Services;
using Habitus.Extensions;
using Habitus.Resources;
using Habitus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Habitus.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HabitsController : ControllerBase
{
    private readonly IHabitService _habitService;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public HabitsController(IHabitService habitService,
                            IAuthService authService,
                            IMapper mapper)
    {
        _habitService = habitService;
        _authService = authService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<HabitResource>> GetHabits()
    {
        var habits = await _habitService.ListAsync();
        var resources = _mapper.Map<IEnumerable<Habit>, IEnumerable<HabitResource>>(habits);

        return Ok(resources);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostHabit([FromBody] SaveHabitResource resource)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.GetErrorMessages());
        }

        var habit = _mapper.Map<SaveHabitResource, Habit>(resource);

        var user = await GetCurrentUserAsync();
        habit.User = user;

        var result = await _habitService.SaveAsync(habit);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var habitResource = _mapper.Map<Habit, HabitResource>(result.Resource);

        return Ok(habitResource);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutHabit(int id, [FromBody] SaveHabitResource resource)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.GetErrorMessages());
        }
        
        var habit = _mapper.Map<SaveHabitResource, Habit>(resource);
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        habit.UserId = userId;
        var result = await _habitService.UpdateAsync(id, habit);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var habitResource = _mapper.Map<Habit, HabitResource>(result.Resource);

        return Ok(habitResource);
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteHabit(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorMessages());

        var result = await _habitService.DeleteAsync(id);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var habitResource = _mapper.Map<Habit, HabitResource>(result.Resource);

        return Ok(habitResource);
    }

    private async Task<HabitusUser> GetCurrentUserAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return await _authService.GetUserByIdAsync(userId);
    }

}
