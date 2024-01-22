using AutoMapper;
using Habitus.Domain.Models;
using Habitus.Domain.Models.Auth;
using Habitus.Domain.Services;
using Habitus.Requests;
using Habitus.Resources;
using Habitus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Habitus.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]

public class HabitsController : ControllerBase
{
    private readonly IHabitService _habitService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public HabitsController(IHabitService habitService,
                            IUserService userService,
                            IMapper mapper)
    {
        _habitService = habitService;
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// List all habits
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HabitResource>), 200)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    [Authorize]
    public async Task<ActionResult<HabitResource>> GetHabits()
    {
        var user = await GetCurrentUser();
        var userId = user.Id;
        var habits = (await _habitService.ListAsync()).Where(h => h.UserId == userId);
        var resources = _mapper.Map<IEnumerable<Habit>, IEnumerable<HabitResource>>(habits);

        return Ok(resources);
    }

    /// <summary>
    /// Add a new habit
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(HabitResource), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    public async Task<IActionResult> PostHabit([FromBody] SaveHabitRequest resource)
    {
        var habit = _mapper.Map<SaveHabitRequest, Habit>(resource);
        var user = await GetCurrentUser();
        habit.User = user;

        var result = await _habitService.SaveAsync(habit);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var habitResource = _mapper.Map<Habit, HabitResource>(result.Resource);

        return Ok(habitResource);
    }

    /// <summary>
    /// Edit a habit
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(HabitResource), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    public async Task<IActionResult> PutHabit(int id, [FromBody] SaveHabitRequest resource)
    {
        var habit = _mapper.Map<SaveHabitRequest, Habit>(resource);
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


    /// <summary>
    /// Delete a habit
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(HabitResource), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    public async Task<IActionResult> DeleteHabit(int id)
    {
        var result = await _habitService.DeleteAsync(id);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var habitResource = _mapper.Map<Habit, HabitResource>(result.Resource);

        return Ok(habitResource);
    }

    private async Task<HabitusUser> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return _userService.GetById(userId).Result.Resource;
    }

}
