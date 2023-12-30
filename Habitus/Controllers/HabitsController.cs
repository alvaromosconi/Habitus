using AutoMapper;
using Habitus.Domain.Models;
using Habitus.Domain.Services;
using Habitus.Extensions;
using Habitus.Resources;
using Habitus.Services;
using Microsoft.AspNetCore.Mvc;


namespace Habitus.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HabitsController : ControllerBase
{
    private readonly IHabitService _habitService;
    private readonly IMapper _mapper;

    public HabitsController(IHabitService habitService,
                            IMapper mapper)
    {
        _habitService = habitService;
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
    public async Task<IActionResult> PostHabit([FromBody] SaveHabitResource resource)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.GetErrorMessages());
        }

        var category = _mapper.Map<SaveHabitResource, Habit>(resource);
        var result = await _habitService.SaveAsync(category);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var categoryResource = _mapper.Map<Habit, HabitResource>(result.Resource);

        return Ok(categoryResource);
    }
}
