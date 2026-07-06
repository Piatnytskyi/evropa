using Evropa.World.Core.Structs.Samplings;
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
        serviceCollection.AddTransient<IPlaneSampler<CircleSamplingRegion>, UniformPoissonSampler<CircleSamplingRegion>>();
        serviceCollection.AddTransient<IPlaneSampler<RectangleSamplingRegion>, UniformPoissonSampler<RectangleSamplingRegion>>();
        serviceCollection.AddTransient<IPlaneSampler<HexagonSamplingRegion>, UniformPoissonSampler<HexagonSamplingRegion>>();
        serviceCollection.AddTransient<IPlaneSampler<PolygonSamplingRegion>, UniformPoissonSampler<PolygonSamplingRegion>>();
        serviceCollection.AddTransient<ITriangulator, DelaunayTriangulator>();
        serviceCollection.AddTransient<IQuadConverter, QuadConverter>();
        serviceCollection.AddTransient<IVoronoiDiagramCalculator, PseudoVoronoiDiagramCalculator>();
        serviceCollection.AddTransient<ISphereSampler, FibonacciSphereSampler>();
        serviceCollection.AddTransient<IStereographicProjector, StereographicProjector>();
        serviceCollection.AddTransient<IFaceSmoother, LaplacianSmoother>();
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}