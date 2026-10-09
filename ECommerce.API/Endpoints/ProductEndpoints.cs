using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.Domain.Common;
using ECommerce.Domain.Constants;
using ECommerce.Infrastructure;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products;
using ECommerce.UseCases.Products.Commands.CreateProduct;
using ECommerce.UseCases.Products.Commands.DeleteProduct;
using ECommerce.UseCases.Products.Commands.UpdateProduct;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Products.Queries.GetByIdProduct;
using ECommerce.UseCases.Products.Queries.GetPagedProducts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints.MapGroup("/api/v{version:apiVersion}/products").WithTags("Products")
                             .WithApiVersionSet(apiVersionSet).HasApiVersion(new ApiVersion(1, 0))
                             .AddEndpointFilter<AuditEndpointFilter>();

        group.MapGet("/paged", async ([AsParameters] GetPagedProductQuery query,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            return result.FromPagedResult(httpContext,query.PageNumber,query.PageSize,"Products retrieved successfully");
        })
        .WithSummary("Get paginated products")
        .WithDescription("Returns a paginated list of products with filtering and sorting options")
        .CacheOutput("Products")
        .Produces<ApiResponse<IReadOnlyList<GetAllProductsResponse>>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id,
            ISender sender,
            HttpContext httpContext
            ,CancellationToken ct) =>
        {
            var result = await sender.Send(new GetByIdProductQuery(id), ct);
            return result.FromResult(httpContext,"Product Id retrieved successfully");
        })
        .WithName("GetProductById")
        .WithSummary("Gets product by ID")
        .WithDescription("Returns a single product by its ID")
        .Produces<ApiResponse<GetByIdProductResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async ([FromForm] CreateProductCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            if (result.IsFailure)
            {
                return result.Problem(httpContext);
            }

            return Results.CreatedAtRoute(
                routeName: "GetProductById",
                routeValues: new { id = result.Value },
                value: ApiResponse<Guid>.Ok(
                    result.Value,
                    httpContext.TraceIdentifier,
                    "Product created successfully"));
        })
            .DisableAntiforgery()
            .WithSummary("Creates a new product")
            .WithDescription("Returns product ID")
            .Accepts<CreateProductCommand>("multipart/form-data")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(p => p.RequireRole(Roles.SuperAdmin));

    group.MapPut("/{id:guid}", async (
    Guid id,
    [FromForm] UpdateProductCommand command,
    ISender sender,
    HttpContext httpContext,
    CancellationToken ct) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(
                    ApiResponse<string>.Fail(
                        "Route ID does not match request ID.",
                        httpContext.TraceIdentifier));
            }

            var result = await sender.Send(command, ct);

            if (result.IsFailure)
                return result.Problem(httpContext);

            return result.FromResult(
                httpContext,
                "Product updated successfully");
        })
        .DisableAntiforgery()
        .WithSummary("Updates a product")
        .WithDescription("Updates product information and optionally replaces its image")
        .Accepts<UpdateProductCommand>("multipart/form-data")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(p => p.RequireRole(Roles.SuperAdmin));

        group.MapDelete("/{id:guid}", async (Guid id,ISender sender,HttpContext httpContext,CancellationToken ct) =>
        {
            var result = await sender.Send(
                new DeleteProductCommand(id),
                ct);

            if (result.IsFailure)
                return result.Problem(httpContext);

            return Results.Ok(
                ApiResponse<string>.Ok(
                    "Product deleted successfully",
                    httpContext.TraceIdentifier));
        })
        .WithSummary("Deletes a product")
        .WithDescription("Soft deletes a product")
        .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(p => p.RequireRole(Roles.SuperAdmin));

        return endpoints;
    }
}
