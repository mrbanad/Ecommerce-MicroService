using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ApiGateway
{
    public static class ExceptionHandlingConfiguration
    {
        private static bool _hasJwtTokenError = false;

        public static async Task HandleGlobalException(HttpContext httpContext, bool includeDetails)
        {
            // Try and retrieve the error from the ExceptionHandler middleware
            var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;

            // Should always exist, but best to be safe!
            if (ex != null)
            {
                // ProblemDetails has it's own content type
                httpContext.Response.ContentType = "application/json";

                // Get the details to display, depending on whether we want to expose the raw exception
                var title = ex.Message;
                var details = includeDetails ? ex.ToString().Replace(Environment.NewLine, "\n") : null;

                var response = new
                {
                    Title = title,
                    Detail = details,
                    TraceId = Activity.Current?.Id ?? httpContext.TraceIdentifier
                };

                //Serialize the problem details object to the Response as JSON (using System.Text.Json)
                var stream = httpContext.Response.Body;
                await JsonSerializer.SerializeAsync(stream, response);
            }
        }

        public static Task OnJwtForbidden(ForbiddenContext context)
        {
            var response = new
            {
                Title = "AccessDenied",
                Detail = "AccessDenied"
            };

            context.Response.ContentType = "application/json";
            context.Response.WriteAsync(JsonConvert.SerializeObject(response));

            return Task.CompletedTask;
        }

        public static Task OnJwtAuthenticationFailed(AuthenticationFailedContext context)
        {
            _hasJwtTokenError = true;
            var response = new
            {
                Title = "JwtTokenExpired",
                Detail = "JwtTokenExpired"
            };

            context.NoResult();
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            context.Response.WriteAsync(JsonConvert.SerializeObject(response)).Wait();

            return Task.CompletedTask;
        }

        public static Task OnJwtChallenge(JwtBearerChallengeContext context)
        {
            if (!_hasJwtTokenError)
                return Task.CompletedTask;

            context.HandleResponse();
            _hasJwtTokenError = false;

            return Task.CompletedTask;
        }
    }
}