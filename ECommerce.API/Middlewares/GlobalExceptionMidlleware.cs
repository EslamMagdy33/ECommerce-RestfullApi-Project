using Microsoft.AspNetCore.Diagnostics;

namespace ECommerce.API.Middlewares;

public class GlobalExceptionMidlleware(IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionMidlleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "UnHandled Exception");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var probemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error ",
                Detail = "An Unexpected error Occurred . Please try again Later"
            }
        };
        return await problemDetailsService.TryWriteAsync(probemDetailsContext);

    }
}
