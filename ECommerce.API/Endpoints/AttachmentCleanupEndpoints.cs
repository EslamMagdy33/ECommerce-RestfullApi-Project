using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Models;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.Interfaces;

namespace ECommerce.API.Endpoints;

public static class AttachmentCleanupEndpoints
{
    public static IEndpointRouteBuilder MapAttachmentCleanupEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/attachments")
            .WithTags("Attachments")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0));

        group.MapPost("/cleanup", async (
            IAttachmentCleanupService cleanupService,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result =
                await cleanupService.CleanupOrphanedAttachmentsAsync(ct);

            if (result.IsFailure)
                return result.Problem(httpContext);

            return Results.Ok(
                ApiResponse<string>.Ok(
                    "Orphaned attachments cleaned up successfully",
                    httpContext.TraceIdentifier));
        })
        .WithSummary("Cleans up orphaned attachments")
        .WithDescription(
            "Deletes Cloudinary attachments that are no longer referenced by any product and removes their database records.")
        .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        return endpoints;
    }
}