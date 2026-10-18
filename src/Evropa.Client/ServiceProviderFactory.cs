using System;
using Evropa.Mathematics;
using Evropa.World.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Evropa.Client;

public static class ServiceProviderFactory
{
    public static IServiceProvider ServiceProvider { get; }

    static ServiceProviderFactory()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMathematics();
        serviceCollection.AddWorldApplication();
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}
