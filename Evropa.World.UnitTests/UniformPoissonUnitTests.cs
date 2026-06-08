namespace Evropa.World.UnitTests;

using System.Numerics;
using Evropa.World.Core.Enums;
using Evropa.World.Core.Structs.Samplings;
using Evropa.World.Infrastructure.Math.Implementations;
using Evropa.World.UnitTests.Data;
using MathNet.Numerics.Random;

public class UniformPoissonUnitTests
{
    private readonly UniformPoissonSampler<CircleSamplingRegion> _circleSampler;
    private readonly UniformPoissonSampler<RectangleSamplingRegion> _rectangleSampler;
    private readonly UniformPoissonSampler<HexagonSamplingRegion> _hexagonSampler;

    public UniformPoissonUnitTests()
    {
        _circleSampler = new UniformPoissonSampler<CircleSamplingRegion>(new MersenneTwister());
        _rectangleSampler = new UniformPoissonSampler<RectangleSamplingRegion>(new MersenneTwister());
        _hexagonSampler = new UniformPoissonSampler<HexagonSamplingRegion>(new MersenneTwister());
    }

    [Theory]
    [ClassData(typeof(UniformPoissonDiskSamplerPositiveData))]
    public void SampleCircle_WithVariousParameters_ReturnsValidPoints(
        Vector2 center, float radius, float minimumDistance, int pointsPerIteration)
    {
        // Arrange
        var region = new CircleSamplingRegion(center, radius, minimumDistance, pointsPerIteration);

        // Act
        var result = _circleSampler.Sample(region);

        // Assert
        Assert.NotNull(result);

        // All points should be within the circle
        Assert.All(result, point =>
        {
            var distance = Vector2.Distance(center, point);
            Assert.True(distance <= radius, 
                $"Point ({point.X}, {point.Y}) is outside circle radius {radius} at distance {distance} from center ({center.X}, {center.Y})");
        });

        // Check that all points are at least minimumDistance apart
        for (int i = 0; i < result.Length; i++)
        {
            for (int j = i + 1; j < result.Length; j++)
            {
                var distance = Vector2.Distance(result[i], result[j]);
                Assert.True(distance >= minimumDistance, 
                    $"Points at index {i} and {j} are closer than minimum distance: {distance} < {minimumDistance}");

                // Check that points are different
                Assert.False(result[i] == result[j], 
                    $"Points at index {i} and {j} are identical: ({result[i].X}, {result[i].Y})");
            }
        }
    }

    [Fact]
    public void SampleHexagon_WithStandardParameters_ReturnsValidPoints()
    {
        // Arrange
        var center = new Vector2(100, 100);
        var radius = 50.0f;
        var minimumDistance = 10.0f;
        var region = new HexagonSamplingRegion(center, radius, minimumDistance, HexagonOrientation.PointyTop, 30);

        // Act
        var result = _hexagonSampler.Sample(region);

        // Assert
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
        // Arrange
        var center = new Vector2(100, 100);
        var radius = 50.0f;
        var minimumDistance = 10.0f;
        var region = new HexagonSamplingRegion(center, radius, minimumDistance, HexagonOrientation.FlatTop, 30);

        // Act
        var result = _hexagonSampler.Sample(region);

        // Assert
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
}