using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Exceptions;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,IProblemDetailsService problemDetailsService )
    : IExceptionHandler 
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred.");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title= "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later."
            }
        };
        return await problemDetailsService.TryWriteAsync(problemDetailsContext);
    }
}
