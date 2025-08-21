namespace Evropa.World.UnitTests;

using System.Numerics;
using Evropa.Infrastructure.Math.Implementations;
using Evropa.World.UnitTests.Data;
using MathNet.Numerics.Random;

public class UniformPoissonDiskUnitTests
{
    private readonly UniformPoissonDiskSampler _uniformPoissonDiskSampler;

    public UniformPoissonDiskUnitTests()
    {
        _uniformPoissonDiskSampler = new UniformPoissonDiskSampler(new MersenneTwister());
    }

    [Theory]
    [ClassData(typeof(UniformPoissonDiskSamplerPositiveData))]
    public void SampleCircle_WithVariousParameters_ReturnsValidPoints(
        Vector2 center, float radius, float minimumDistance, int pointsPerIteration)
    {
        // Act
        var result = _uniformPoissonDiskSampler.SampleCircle(center, radius, minimumDistance, pointsPerIteration);

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
        for (int i = 0; i < result.Count; i++)
        {
            for (int j = i + 1; j < result.Count; j++)
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
}