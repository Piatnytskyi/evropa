namespace Evropa.World.UnitTests;

using Evropa.Infrastructure.Math.Implementations;
using MathNet.Numerics.Random;

public class PoissonDiskSamplingFacadeUnitTests
{
    private readonly PoissonDiskSamplingFacade _facade;

    public PoissonDiskSamplingFacadeUnitTests()
    {
        _facade = new PoissonDiskSamplingFacade(new MersenneTwister());
    }

    [Theory]
    [InlineData(100, 100, 5.0f, 10)]
    [InlineData(100, 100, 5.0f, 0)]
    [InlineData(50, 30, 2.0f, 20)]
    [InlineData(100, 100, 1.0f, 1)]
    [InlineData(200, 150, 3.0f, 5)]
    [InlineData(500, 300, 7.0f, 50)]
    public void GeneratePoissonDiskSampling_WithVariousParameters_ReturnsExpectedResults(
        float width, float height, float minDistance, int k)
    {
        // Act
        var result = _facade.GeneratePoissonDiskSampling(width, height, minDistance, k);

        // Assert
        Assert.NotNull(result);

        Assert.All(result, point =>
        {
            Assert.True(point.X >= 0 && point.X <= width, $"Point X coordinate {point.X} is out of bounds [0, {width})");
            Assert.True(point.Y >= 0 && point.Y <= height, $"Point Y coordinate {point.Y} is out of bounds [0, {height})");
        });

        // Check that all points are at least minDistance apart
        for (int i = 0; i < result.Length; i++)
        {
            for (int j = i + 1; j < result.Length; j++)
            {
                var dx = result[i].X - result[j].X;
                var dy = result[i].Y - result[j].Y;
                var distance = Math.Sqrt(dx * dx + dy * dy);
                Assert.True(distance >= minDistance, $"Points at index {i} and {j} are closer than minDistance: {distance} < {minDistance}");

                // Check that points are different
                Assert.False(result[i].X == result[j].X && result[i].Y == result[j].Y, $"Points at index {i} and {j} are identical: ({result[i].X}, {result[i].Y})");
            }
        }
    }
}
