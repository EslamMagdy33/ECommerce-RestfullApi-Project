using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using ECommerce.API;
using ECommerce.API.Endpoints;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.persistence.Data.DbContexts;
using ECommerce.Infrastructure.Persistence.Data.Interceptor;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.UseCases;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("Products", policy =>
    {
        policy.Expire(TimeSpan.FromMinutes(1))
        .SetVaryByQuery("Pagesize", "PageNumber", "Search", "BrandId", "TypeId", "SortBy", "SortDescending");
    });
});


builder.Services.AddPresentation();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();


var app = builder.Build();

app.UseExceptionHandler();

app.UseOutputCache();

app.UseAuthentication();

app.UseAuthorization();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

app.MapProductEndpoints(apiVersionSet);
app.MapBasketEndpoints(apiVersionSet);
app.MapUserEndpoints(apiVersionSet);
app.MapAuthEndpoints(apiVersionSet);
app.MapTypeEndpoints(apiVersionSet);
app.MapBrandEndpoints(apiVersionSet);
app.MapDeliveryMethodEndpoints(apiVersionSet);
app.MapOrderEndpoints(apiVersionSet);
app.MapPaymentEndpoints(apiVersionSet);

if (app.Environment.IsDevelopment())
{
    #region Enable Swagger Middleware
    app.UseSwagger();
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });

    #endregion

    #region Seeding
    await using var scope = app.Services.CreateAsyncScope();
    var sp = scope.ServiceProvider;

    var identityDb = sp.GetRequiredService<AppIdentityDbContext>();
    await identityDb.Database.MigrateAsync();

    var appDb = sp.GetRequiredService<StoreDbContext>();
    await appDb.Database.MigrateAsync();

    await sp.GetRequiredService<DatabaseSeeder>().SeadAll();

    #endregion
}


await app.RunAsync();

