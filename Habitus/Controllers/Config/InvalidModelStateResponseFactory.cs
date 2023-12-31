using Habitus.Extensions;
using Habitus.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Habitus.Controllers.Config;

public static class InvalidModelStateResponseFactory
{
    public static IActionResult ProduceErrorResponse(ActionContext context)
    {
        var errors = context.ModelState.GetErrorMessages();
        var response = new ErrorResource(messages: errors);

        return new BadRequestObjectResult(response);
    }
}