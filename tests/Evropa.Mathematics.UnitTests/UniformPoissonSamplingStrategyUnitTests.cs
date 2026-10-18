namespace Evropa.Mathematics.UnitTests;

using System.Numerics;
using Evropa.Mathematics.Sampling;
using Evropa.Mathematics.Sampling.Regions;
using Evropa.Mathematics.UnitTests.Data;
using MathNet.Numerics.Random;

public class UniformPoissonSamplingStrategyUnitTests
{
    private readonly ISampler<(CircleSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector2> _circleSampler;
    private readonly ISampler<(HexagonSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector2> _hexagonSampler;
    private readonly ISampler<(SphereSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector3> _sphereSampler;
    private readonly ISampler<(BoxSamplingRegion Region, float MinimumDistance, int PointsPerIteration), Vector3> _boxSampler;

    public UniformPoissonSamplingStrategyUnitTests()
    {
        _circleSampler = new Sampler<(CircleSamplingRegion, float, int), Vector2>(
            new UniformPoissonSamplingStrategy<CircleSamplingRegion, Vector2>(new MersenneTwister()));
        _hexagonSampler = new Sampler<(HexagonSamplingRegion, float, int), Vector2>(
            new UniformPoissonSamplingStrategy<HexagonSamplingRegion, Vector2>(new MersenneTwister()));
        _sphereSampler = new Sampler<(SphereSamplingRegion, float, int), Vector3>(
            new UniformPoissonSamplingStrategy<SphereSamplingRegion, Vector3>(new MersenneTwister()));
        _boxSampler = new Sampler<(BoxSamplingRegion, float, int), Vector3>(
            new UniformPoissonSamplingStrategy<BoxSamplingRegion, Vector3>(new MersenneTwister()));
    }

    [Theory]
    [ClassData(typeof(UniformPoissonPlaneSamplingStrategyPositiveData))]
    public void SampleCircle_WithVariousParameters_ReturnsValidPoints(
        Vector2 center, float radius, float minimumDistance, int pointsPerIteration)
    {
        var region = new CircleSamplingRegion(center, radius);
        var result = _circleSampler.Sample((region, minimumDistance, pointsPerIteration));

        Assert.NotNull(result);
        Assert.All(result, point =>
        {
            var distance = Vector2.Distance(center, point);
            Assert.True(distance <= radius,
                $"Point ({point.X}, {point.Y}) is outside circle radius {radius} at distance {distance} from center ({center.X}, {center.Y})");
        });

        for (int i = 0; i < result.Length; i++)
        {
            for (int j = i + 1; j < result.Length; j++)
            {
                var distance = Vector2.Distance(result[i], result[j]);
                Assert.True(distance >= minimumDistance,
                    $"Points at index {i} and {j} are closer than minimum distance: {distance} < {minimumDistance}");
                Assert.False(result[i] == result[j],
                    $"Points at index {i} and {j} are identical: ({result[i].X}, {result[i].Y})");
            }
        }
    }

    [Fact]
    public void SampleHexagon_WithStandardParameters_ReturnsValidPoints()
    {
        var center = new Vector2(100, 100);
        var radius = 50.0f;
        var minimumDistance = 10.0f;
        var region = new HexagonSamplingRegion(center, radius, HexagonOrientation.PointyTop);

        var result = _hexagonSampler.Sample((region, minimumDistance, 30));

        Assert.NotNull(result);
        Assert.All(result, point =>
        {
            Assert.True(region.Contains(point),
                $"Point ({point.X}, {point.Y}) is outside hexagon");
        });

        for (int i = 0; i < result.Length; i++)
        {
            for (int j = i + 1; j < result.Length; j++)
            {
                var distance = Vector2.Distance(result[i], result[j]);
                Assert.True(distance >= minimumDistance,
                    $"Points at index {i} and {j} are closer than minimum distance: {distance} < {minimumDistance}");
            }
        }
    }

    [Fact]
    public void SampleHexagon_FlatTop_ReturnsValidPoints()
    {
        var center = new Vector2(100, 100);
        var radius = 50.0f;
        var minimumDistance = 10.0f;
        var region = new HexagonSamplingRegion(center, radius, HexagonOrientation.FlatTop);

        var result = _hexagonSampler.Sample((region, minimumDistance, 30));

        Assert.NotNull(result);
        Assert.All(result, point =>
        {
            Assert.True(region.Contains(point),
                $"Point ({point.X}, {point.Y}) is outside hexagon");
        });

        for (int i = 0; i < result.Length; i++)
        {
            for (int j = i + 1; j < result.Length; j++)
            {
                var distance = Vector2.Distance(result[i], result[j]);
                Assert.True(distance >= minimumDistance,
                    $"Points at index {i} and {j} are closer than minimum distance: {distance} < {minimumDistance}");
            }
        }
    }

    [Theory]
    [ClassData(typeof(UniformPoissonVolumeSamplingStrategyPositiveData))]
    public void SampleSphere_WithVariousParameters_ReturnsValidPoints(
        Vector3 center, float radius, float minimumDistance, int pointsPerIteration)
    {
        var region = new SphereSamplingRegion(center, radius);
        var result = _sphereSampler.Sample((region, minimumDistance, pointsPerIteration));

        Assert.NotNull(result);
        Assert.All(result, point =>
        {
            var distance = Vector3.Distance(center, point);
            Assert.True(distance <= radius,
                $"Point ({point.X}, {point.Y}, {point.Z}) is outside sphere radius {radius} at distance {distance}");
        });

        for (int i = 0; i < result.Length; i++)
        {
            for (int j = i + 1; j < result.Length; j++)
            {
                var distance = Vector3.Distance(result[i], result[j]);
                Assert.True(distance >= minimumDistance,
                    $"Points at index {i} and {j} are closer than minimum distance: {distance} < {minimumDistance}");
            }
        }
    }

    [Fact]
    public void SampleBox_WithStandardParameters_ReturnsValidPoints()
    {
        var min = new Vector3(0, 0, 0);
        var max = new Vector3(50, 50, 50);
        var minimumDistance = 10.0f;
        var region = new BoxSamplingRegion(min, max);

        var result = _boxSampler.Sample((region, minimumDistance, 30));

        Assert.NotNull(result);
        Assert.All(result, point =>
        {
            Assert.True(region.Contains(point),
                $"Point ({point.X}, {point.Y}, {point.Z}) is outside box");
        });

        for (int i = 0; i < result.Length; i++)
        {
            for (int j = i + 1; j < result.Length; j++)
            {
                var distance = Vector3.Distance(result[i], result[j]);
                Assert.True(distance >= minimumDistance,
                    $"Points at index {i} and {j} are closer than minimum distance: {distance} < {minimumDistance}");
            }
        }
    }
}
