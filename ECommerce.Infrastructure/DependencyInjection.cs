using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Payments;
using ECommerce.Infrastructure.persistence.Data.DbContexts;
using ECommerce.Infrastructure.persistence.Seeding;
using ECommerce.Infrastructure.Persistence.Data.Interceptor;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ECommerce.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(connectionString,sql=>
                sql.MigrationsHistoryTable("__ApplicationMigrationsHistory","app"))
                        .EnableSensitiveDataLogging();
            });
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(connectionString,sql =>
                sql.MigrationsHistoryTable("__IdentityMigrationsHistory","identity"))
                        .EnableSensitiveDataLogging();
            });

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));

            var jwtSettings = config.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ??
                throw new InvalidOperationException($"Configuration section '{JwtSettings.SectionName}' is missing.");

            if (string.IsNullOrEmpty(jwtSettings.Secret) || jwtSettings.Secret.Length < 32)
                throw new InvalidOperationException("Jwt:Secret must be at least 32 characters.");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // هامش سماح للوقت يعني بيدي ال Token فرصه قبل ما ي expire 
                };
            });
            services.AddAuthorization();
            services.AddHttpContextAccessor();

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IJwtGenerator, JwtTokenGenerator>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailVerificationCodeStore, HybridEmailVerificationCodeStore>();

            services.Configure<EmailVerificationSettings>(
            config.GetSection(EmailVerificationSettings.SectionName));

            services.Configure<EmailSettings>(config.GetSection(EmailSettings.SectionName));
            var emailSettings = config.GetSection(EmailSettings.SectionName).Get<EmailSettings>()
                ?? throw new InvalidOperationException(
                    $"Configuration section '{EmailSettings.SectionName}' is missing.");

            services.Configure<StripeSettings>(config.GetSection(StripeSettings.SectionName));
            var stripeSettings = config.GetSection(StripeSettings.SectionName).Get<EmailSettings>()
                ?? throw new InvalidOperationException(
                    $"Configuration section '{StripeSettings.SectionName}' is missing.");

            services
                .AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName)
                .AddSmtpSender(emailSettings.Host, emailSettings.Port);

            services.AddScoped<IEmailSender, FluentEmailSender>();

            services.AddScoped<ISoftDeleteInterceptor, SoftDeleteInterceptor>();
            services.AddScoped<IAuditInterceptor, AuditInterceptor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IDataSeeder, ProductBrandSeeder>();
            services.AddScoped<IDataSeeder, ProductTypeSeeder>();
            services.AddScoped<IDataSeeder, IdentitySeeder>();
            services.AddScoped<DatabaseSeeder>();
            services.AddScoped<IDataSeeder, DeliveryMethodSeeder>();

            services.Configure<CloudinarySettings>(config.GetSection(nameof(CloudinarySettings)));
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IPaymentService, StripePaymentService>();

            AddBasketCaching(services, config);

            return services;
        }
        private static void AddBasketCaching(IServiceCollection services, IConfiguration config)
        {
            services
                .AddOptions<CacheEntryPolicy>("Basket")
                .Bind(config.GetSection("CachedAggregate:Basket"))
                .ValidateOnStart();
            services.AddSingleton<IValidateOptions<CacheEntryPolicy>, CacheEntryPolicyValidator>();

            var redisConnection = config.GetConnectionString("Redis") ??
                config.GetConnectionString("redis") ??
                throw new InvalidOperationException("Connection string 'Redis' is missing in appsettings.json."); 
            if(!string.IsNullOrEmpty(redisConnection))
            {
                services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
            }
            services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = 1024 * 1024;
            });
            services.AddScoped(typeof(ICachedAggregateStore<>), typeof(HybridCachedAggregateStore<>));
            services.AddScoped<IBasketStore, HybridBasketStore>();

        }
    }

}
