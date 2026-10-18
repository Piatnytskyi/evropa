namespace Evropa.Mathematics;

using System.Numerics;
using Evropa.Mathematics.Projection;
using Evropa.Mathematics.Sampling;
using Evropa.Mathematics.Sampling.Regions;
using Evropa.Mathematics.Smoothing;
using Evropa.Mathematics.Tessellation;
using Evropa.Mathematics.Triangulation;
using MathNet.Numerics.Random;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddMathematics(this IServiceCollection services)
    {
        services.AddTransient<RandomSource, MersenneTwister>();

        services.AddTransient<ISamplingStrategy<(CircleSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector2>, UniformPoissonSamplingStrategy<CircleSamplingRegion, Vector2>>();
        services.AddTransient<ISamplingStrategy<(RectangleSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector2>, UniformPoissonSamplingStrategy<RectangleSamplingRegion, Vector2>>();
        services.AddTransient<ISamplingStrategy<(HexagonSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector2>, UniformPoissonSamplingStrategy<HexagonSamplingRegion, Vector2>>();
        services.AddTransient<ISamplingStrategy<(PolygonSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector2>, UniformPoissonSamplingStrategy<PolygonSamplingRegion, Vector2>>();

        services.AddTransient<ISamplingStrategy<(SphereSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector3>, UniformPoissonSamplingStrategy<SphereSamplingRegion, Vector3>>();
        services.AddTransient<ISamplingStrategy<(BoxSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector3>, UniformPoissonSamplingStrategy<BoxSamplingRegion, Vector3>>();

        services.AddTransient<ISamplingStrategy<(int Count, float Jitter), Vector3>, FibonacciSphereSamplingStrategy>();
        services.AddTransient(typeof(ISampler<,>), typeof(Sampler<,>));
        services.AddTransient<ITriangulator, DelaunayTriangulator>();
        services.AddTransient<IQuadConverter, QuadConverter>();
        services.AddTransient<IVoronoiDiagramCalculator, PseudoVoronoiDiagramCalculator>();
        services.AddTransient<IStereographicProjector, StereographicProjector>();
        services.AddTransient<IFaceSmoother, LaplacianSmoother>();

        return services;
    }
}
