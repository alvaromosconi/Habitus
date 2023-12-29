using Habitus.Domain.Services;
using Microsoft.AspNetCore.Mvc;


namespace Habitus.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HabitsController : ControllerBase
{
    private readonly HabitusContext _context;
    private readonly IHabitService _habitService;

    public HabitsController(HabitusContext context, 
                            IHabitService habitService)
    {
        _context = context;
        _habitService = habitService;
    }

    // GET: api/Habits
}
