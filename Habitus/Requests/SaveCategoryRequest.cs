using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;

namespace Habitus.Requests;

/// <summary>
/// Represents the data required to create or update a category.
/// </summary>
public record SaveCategoryRequest
{
    /// <summary>
    /// The category name.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string? Name { get; init; }
}

/// <summary>
/// Provides examples of the <see cref="SaveCategoryRequest"/> for Swagger documentation.
/// </summary>
public class SaveCategoryRequestExample : IExamplesProvider<SaveCategoryRequest>
{
    /// <summary>
    /// Gets examples of the <see cref="SaveCategoryRequest"/>.
    /// </summary>
    /// <returns>An example of the <see cref="SaveCategoryRequest"/>.</returns>
    public SaveCategoryRequest GetExamples()
    {
        return new SaveCategoryRequest
        {
            Name = "Lifestyle"
        };
    }
}