using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheStore.ProductManagement.API;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.Tests;

public class ValidateModelFilterTests
{
    private readonly ValidateModelFilter _filter;

    public ValidateModelFilterTests()
    {

        _filter = new ValidateModelFilter();
    }

    [Fact]
    public void OnActionExecuting_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "Product Name is required");

        var mockHttpContext = new Mock<HttpContext>();
        var mockActionContext = new ActionContext(
            mockHttpContext.Object,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ControllerActionDescriptor(),
            modelState
        );

        var mockActionExecutingContext = new ActionExecutingContext(
            mockActionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            new Mock<Controller>().Object
        );

        // Act
        _filter.OnActionExecuting(mockActionExecutingContext);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(mockActionExecutingContext.Result);
        var response = Assert.IsType<Error>(badRequestResult.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Contains("Product Name is required", response.Message);
    }
}
