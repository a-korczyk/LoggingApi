using System.Text;
using LoggingApi.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace LoggingApi.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddProblemDetails(); 
        
        services.AddHttpContextAccessor();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddOptions<AccessTokenOptions>()
            .BindConfiguration(AccessTokenOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<RefreshTokenOptions>()
            .BindConfiguration(RefreshTokenOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var accessTokenOptions = configuration
            .GetSection(AccessTokenOptions.SectionName)
            .Get<AccessTokenOptions>() ?? throw new InvalidOperationException("Access token configuration not found.");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = accessTokenOptions.Issuer,
                    ValidateIssuer = true,
            
                    ValidAudience = accessTokenOptions.Audience,
                    ValidateAudience = true,
            
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(accessTokenOptions.Secret)),
            
                    ValidateIssuerSigningKey = true,
            
                    ValidateLifetime = true,
                };
            });
        
        services.AddAuthorization();
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "LoggingApi API",
                Description = "An ASP.NET Core Web API for logging logs",
            });
    
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT",
            });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
        });
        
        return services;
    }
}
