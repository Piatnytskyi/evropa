using Evropa.Infrastructure.Math.Abstractions;
using Evropa.Infrastructure.Math.Implementations;
using MathNet.Numerics.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Evropa.Infrastructure;

public static class ServiceProviderFactory
{
    public static IServiceProvider ServiceProvider { get; }

    static ServiceProviderFactory()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<RandomSource, MersenneTwister>();
        serviceCollection.AddTransient<IDiskSampler, UniformPoissonDiskSampler>();
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}