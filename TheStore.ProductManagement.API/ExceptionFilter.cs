using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Text.Json;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IDatabaseService _dbService;
        public ExceptionFilter(IDatabaseService dbService)
        {
                _dbService = dbService;
        }
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            
            int statusCode = StatusCodes.Status500InternalServerError;
            string errorMessages = context.Exception?.Message ?? "An unexpected error occurred";

            var error = new Error(statusCode, new List<string?> { errorMessages });
     


            var errorLog = new ApiErrorLog
            {
                IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Endpoint = context.HttpContext.Request.Path.ToString(),
                RequestTimestamp = DateTime.UtcNow,
                RequestMethod = context.HttpContext.Request.Method,
                StatusCode = error.StatusCode,
                Message = error.Message
            };

            await _dbService.ApiErrorSaveToDb(errorLog);

            context.Result = new JsonResult(error) { StatusCode = statusCode };
            context.ExceptionHandled = true;
        }

    }

}
