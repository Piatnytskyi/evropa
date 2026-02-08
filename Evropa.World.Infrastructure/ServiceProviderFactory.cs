using Evropa.World.Infrastructure.Math.Abstractions;
using Evropa.World.Infrastructure.Math.Implementations;
using MathNet.Numerics.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Evropa.World.Infrastructure;

public static class ServiceProviderFactory
{
    public static IServiceProvider ServiceProvider { get; }

    static ServiceProviderFactory()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<RandomSource, MersenneTwister>();
        serviceCollection.AddTransient<IDiskSampler, UniformPoissonDiskSampler>();
        serviceCollection.AddTransient<ITriangulation, DelaunayTriangulation>();
        serviceCollection.AddTransient<IFibonacciSphereGenerator, FibonacciSphereGenerator>();
        serviceCollection.AddTransient<IStereographicProjection, StereographicProjection>();
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}