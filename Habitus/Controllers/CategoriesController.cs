using Microsoft.AspNetCore.Mvc;
using Habitus.Models;
using Habitus.Services;

namespace Habitus.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<Category>> GetCategories()
    {
        var categories = await _categoryService.ListAsync();
        
        return Ok(categories);
    }
}
