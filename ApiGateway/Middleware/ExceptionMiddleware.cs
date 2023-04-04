using System.Net;

namespace ApiGateway.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception here
            Console.WriteLine(ex.Message);

            // Return a meaningful error response to the client
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            await context.Response.WriteAsync(new
            {
                ErrorMessage = "An error occurred while processing your request."
            }.ToString() ?? string.Empty);
        }
    }
}