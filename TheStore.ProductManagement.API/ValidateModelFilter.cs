using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API;

public class ValidateModelFilter: ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {

        context.HttpContext.Request.EnableBuffering();

        if (!context.ModelState.IsValid)
        {
            var errorMessages = context.ModelState
                .SelectMany(v => v.Value.Errors.Select(e => e.ErrorMessage))
                .ToList();

            var response = new Error
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = errorMessages
            };

            context.Result = new BadRequestObjectResult(response);
        }

    }

}



