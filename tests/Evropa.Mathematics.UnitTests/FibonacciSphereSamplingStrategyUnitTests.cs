namespace Evropa.Mathematics.UnitTests;

using System;
using System.Numerics;
using Evropa.Mathematics.Sampling;
using Evropa.Mathematics.UnitTests.Data;
using MathNet.Numerics.Random;

public class FibonacciSphereSamplingStrategyUnitTests
{
    private const float UnitSphereTolerance = 1e-4f;

    [Fact]
    public void Constructor_WithNullRandomSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FibonacciSphereSamplingStrategy(null!));
    }

    [Fact]
    public void Sampler_WithNullStrategy_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Sampler<(int Count, float Jitter), Vector3>(null!));
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereSamplingStrategyPositiveData))]
    public void Sample_ReturnsRequestedNumberOfPoints(int count, float jitter)
    {
        var result = CreateSampler(seed: 42).Sample((count, jitter));

        Assert.NotNull(result);
        Assert.Equal(count, result.Length);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereSamplingStrategyPositiveData))]
    public void Sample_AllPointsLieOnUnitSphere(int count, float jitter)
    {
        var result = CreateSampler(seed: 42).Sample((count, jitter));

        Assert.All(result, point =>
        {
            float length = point.Length();
            Assert.True(
                MathF.Abs(length - 1f) < UnitSphereTolerance,
                $"Point ({point.X}, {point.Y}, {point.Z}) has length {length}, expected 1.");
        });
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereSamplingStrategyPositiveData))]
    public void Sample_WithoutJitter_IsDeterministic(int count, float jitter)
    {
        _ = jitter;

        var samplerA = CreateSampler(seed: 1);
        var samplerB = CreateSampler(seed: 2);

        var resultA = samplerA.Sample((count, 0f));
        var resultB = samplerB.Sample((count, 0f));

        Assert.Equal(resultA, resultB);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereSamplingStrategyPositiveData))]
    public void Sample_WithSameSeedAndJitter_IsDeterministic(int count, float jitter)
    {
        _ = jitter;

        var samplerA = CreateSampler(seed: 123);
        var samplerB = CreateSampler(seed: 123);

        var resultA = samplerA.Sample((count, 0.5f));
        var resultB = samplerB.Sample((count, 0.5f));

        Assert.Equal(resultA, resultB);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereSamplingStrategyPositiveData))]
    public void Sample_WithJitter_DiffersFromUnJittered(int count, float jitter)
    {
        _ = jitter;

        var unjittered = CreateSampler(seed: 7).Sample((count, 0f));
        var jittered = CreateSampler(seed: 7).Sample((count, 0.5f));

        Assert.NotEqual(unjittered, jittered);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereSamplingStrategyPositiveData))]
    public void Sample_PointsAreUnique(int count, float jitter)
    {
        var result = CreateSampler(seed: 42).Sample((count, jitter));

        var unique = new HashSet<Vector3>(result);
        Assert.Equal(count, unique.Count);
    }

    [Fact]
    public void Sample_CalledMultipleTimes_ReusesRandomBuffersWithoutError()
    {
        var sampler = CreateSampler(seed: 42);

        var result1 = sampler.Sample((50, 0.5f));
        var result2 = sampler.Sample((100, 0.5f));
        var result3 = sampler.Sample((25, 0.5f));

        Assert.Equal(50, result1.Length);
        Assert.Equal(100, result2.Length);
        Assert.Equal(25, result3.Length);
    }

    [Fact]
    public void Sample_WithZeroCount_ReturnsEmptyArray()
    {
        var result = CreateSampler(seed: 42).Sample((0, 0f));

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private static ISampler<(int Count, float Jitter), Vector3> CreateSampler(int seed) =>
        new Sampler<(int Count, float Jitter), Vector3>(
            new FibonacciSphereSamplingStrategy(new MersenneTwister(seed: seed)));
}
