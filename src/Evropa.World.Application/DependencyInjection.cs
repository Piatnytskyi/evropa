namespace Evropa.World.Application;

using Evropa.World.Application.Planets;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWorldApplication(this IServiceCollection services)
    {
        services.AddTransient<IPlanetSurfaceGenerator, PlanetSurfaceGenerator>();

        return services;
    }
}
