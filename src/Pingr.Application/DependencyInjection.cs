using FluentValidation;
using Pingr.Application.Behaviors;
using Pingr.Application.Features.Authentication.Login;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Pingr.Application;

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