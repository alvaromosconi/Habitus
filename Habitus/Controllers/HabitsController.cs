using AutoMapper;
using Habitus.Domain.Models;
using Habitus.Domain.Services;
using Habitus.Extensions;
using Habitus.Resources;
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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _authService.GetUserByIdAsync(userId);

        var habit = _mapper.Map<SaveHabitResource, Habit>(resource);

        habit.User = user;

        var result = await _habitService.SaveAsync(habit);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var habitResource = _mapper.Map<Habit, HabitResource>(result.Resource);

        return Ok(habitResource);
    }

}
