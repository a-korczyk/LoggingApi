using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Application.Abstractions.Services.Email;
using LoggingApi.Infrastructure.Repositories;
using LoggingApi.Infrastructure.Services;
using LoggingApi.Infrastructure.Services.Authentication;
using LoggingApi.Infrastructure.Services.Authentication.PasswordHasher;
using LoggingApi.Infrastructure.Services.Logs;
using LoggingApi.Infrastructure.Services.Logs.Digest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using QRCoder;
using StackExchange.Redis;

namespace LoggingApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ => 
            ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("Cache")
                ?? throw new InvalidOperationException("Redis connection string not found.")));
        services.AddSingleton<ICacheService, CacheService>();
        
        // AI chat
        services.AddSingleton<IChatClient>(_ =>
            new OllamaApiClient(
                new HttpClient
                {
                    BaseAddress = new Uri(configuration["Ai:BaseAddress"]),
                    Timeout = TimeSpan.FromMinutes(5)
                },
                configuration["Ai:Model"]));
        services.AddSingleton<IChatService, ChatService>();
        
        // Email
        services.AddSingleton<IEmailSender, EmailSender>();
        services.Configure<EmailOptions>(
            configuration.GetSection(EmailOptions.SectionName));
        services.AddScoped<ILogNotificationService, LogNotificationService>();
        
        // User
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.Configure<PasswordHasherOptions>(
            configuration.GetSection("PasswordHasher"));
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IEmailVerificationRequestRepository, EmailVerificationRequestRepository>();
        
        // 2FA
        services.AddScoped<ITwoFactorChallengeRepository, TwoFactorChallengeRepository>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();
        
        // Refresh token
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // Logs
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddSingleton<ILogDigestQueue, LogDigestQueue>();
        services.AddSingleton<ILogDigestStatisticsBuilder, LogDigestStatisticsBuilder>();
        services.AddSingleton<ILogDigestEmailBuilder, LogDigestEmailBuilder>();
        services.AddHostedService<LogDigestBackgroundService>();
        
        return services;
    }
}
