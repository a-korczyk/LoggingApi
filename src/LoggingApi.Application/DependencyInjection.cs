using FluentValidation;
using LoggingApi.Application.Behaviors;
using LoggingApi.Application.Features.Authentication.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LoggingApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
        });
        
        services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }
}