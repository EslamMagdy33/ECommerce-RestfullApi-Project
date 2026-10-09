using Asp.Versioning;
using Asp.Versioning.Builder;
using CloudinaryDotNet;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.Domain.Constants;
using ECommerce.UseCases.DeliveryMethods.Commands.CreateDeliveryMethod;
using ECommerce.UseCases.DeliveryMethods.Commands.DeleteDeliveryMethod;
using ECommerce.UseCases.DeliveryMethods.Commands.UpdateDeliveryMethod;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethodById;
using ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethods;
using ECommerce.UseCases.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class DeliveryMethodEndpoints
{
    public static IEndpointRouteBuilder MapDeliveryMethodEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/delivery-methods")
            .WithTags("DeliveryMethods")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .AddEndpointFilter<AuditEndpointFilter>()
            .RequireAuthorization(r => r.RequireRole(Roles.SuperAdmin));

        group.MapGet("/", async (
            [FromQuery] bool? availableOnly, [FromQuery] string ? search,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetDeliveryMethodsQuery(availableOnly ?? true,search),
                ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodsRetrieved);
        })
        .WithSummary("List delivery methods")
        .WithDescription("Returns delivery methods. Optionally filter to available ones only.")
        .Produces<ApiResponse<IReadOnlyList<DeliveryMethodResponse>>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetDeliveryMethodByIdQuery(id), ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodRetrieved);
        })
        .WithSummary("Get delivery method by id")
        .Produces<ApiResponse<DeliveryMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async (
            [FromBody] CreateDeliveryMethodCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodCreated);
        })
        .WithSummary("Create delivery method")
        .Produces<ApiResponse<DeliveryMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateDeliveryMethodRequest request,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var command = new UpdateDeliveryMethodCommand(
                id,
                request.Name,
                request.Price,
                request.EstimatedDeliveryTime,
                request.Description,
                request.IsAvailable,
                request.DisplayOrder);

            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodUpdated);
        })
        .WithSummary("Update delivery method")
        .Produces<ApiResponse<DeliveryMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteDeliveryMethodCommand(id), ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodDeleted);
        })
        .WithSummary("Delete delivery method")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}

public sealed record UpdateDeliveryMethodRequest(
    string Name,
    decimal Price,
    string EstimatedDeliveryTime,
    string? Description = null,
    bool IsAvailable = true,
    int DisplayOrder = 0);