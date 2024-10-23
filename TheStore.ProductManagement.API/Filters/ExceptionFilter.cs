using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data.SqlClient;
using TheStore.ProductManagement.API.Database;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API.Filters;

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

        Error error = new Error(statusCode, new List<string?> { errorMessages });

        try
        {
            var errorLog = new ApiErrorLog
            {
                IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Endpoint = context.HttpContext.Request.Path.ToString(),
                RequestTimestamp = DateTime.Now,
                RequestMethod = context.HttpContext.Request.Method,
                StatusCode = error.StatusCode,
                Message = error.Message
            };

            await _dbService.ApiErrorSaveToDb(errorLog);
        }
        catch (SqlException ex)
        {
            errorMessages = "There was a problem connecting to the database. Try again later.";
            error = new Error(statusCode, new List<string?> { errorMessages });
        }
        finally
        {
            context.Result = new JsonResult(error) { StatusCode = statusCode };       
            context.ExceptionHandled = true;
        }
        

    }

}
