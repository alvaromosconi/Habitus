using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Habitus.Resources;
using Habitus.Extensions;
using Habitus.Domain.Services;
using Habitus.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Filters;
using Habitus.Requests;

namespace Habitus.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoriesController(ICategoryService categoryService,
                                IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    /// <summary>
    /// Lists all categories.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(CategoryResource), 200)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    public async Task<ActionResult<CategoryResource>> GetCategories()
    {
        var categories = await _categoryService.ListAsync();
        var resources = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResource>>(categories);

        return Ok(resources);
    }

    /// <summary>
    /// Add a new category
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CategoryResource), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    public async Task<IActionResult> PostCategory([FromBody] SaveCategoryRequest resource)
    {
        var category = _mapper.Map<SaveCategoryRequest, Category>(resource);
        var result = await _categoryService.SaveAsync(category);

        if (!result.Success)
            return BadRequest(result.Message);

        var categoryResource = _mapper.Map<Category, CategoryResource>(result.Resource);

        return Ok(categoryResource);
    }

    /// <summary>
    /// Edit a category
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(CategoryResource), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    [SwaggerRequestExample(typeof(SaveCategoryRequest), typeof(SaveCategoryRequestExample))]
    public async Task<IActionResult> PutCategory(int id, [FromBody] SaveCategoryRequest resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorMessages());

        var category = _mapper.Map<SaveCategoryRequest, Category>(resource);
        var result = await _categoryService.UpdateAsync(id, category);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var categoryResource = _mapper.Map<Category, CategoryResource>(result.Resource);

        return Ok(categoryResource);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CategoryResource), 201)]
    [ProducesResponseType(typeof(ErrorResource), 400)]
    [ProducesResponseType(typeof(ErrorResource), 401)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorMessages());

        var result = await _categoryService.DeleteAsync(id);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var categoryResource = _mapper.Map<Category, CategoryResource>(result.Resource);

        return Ok(categoryResource);
    }

}
