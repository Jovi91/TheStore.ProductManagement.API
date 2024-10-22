using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Quic;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API.Authentication
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthFilter(IConfiguration configuration)
        {
                _configuration = configuration;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey)) {
                var result = new Error
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = new List<string?> { "API Key missing" }
                };
                context.Result = new UnauthorizedObjectResult(result);
                return;
            }

            var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
            if (!apiKey.Equals(extractedApiKey))
            {
                var result = new Error
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = new List<string?> { "Invalid API Key" }
                };
                context.Result = new UnauthorizedObjectResult(result);
                return;
            }
        }
    }
}
