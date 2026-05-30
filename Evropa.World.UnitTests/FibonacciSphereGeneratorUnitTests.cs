namespace Evropa.World.UnitTests;

using System;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Implementations;
using Evropa.World.UnitTests.Data;
using MathNet.Numerics.Random;

public class FibonacciSphereGeneratorUnitTests
{
    private const float UnitSphereTolerance = 1e-4f;

    private readonly FibonacciSphereGenerator _generator;

    public FibonacciSphereGeneratorUnitTests()
    {
        _generator = new FibonacciSphereGenerator(new MersenneTwister(seed: 42));
    }

    [Fact]
    public void Constructor_WithNullRandomSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FibonacciSphereGenerator(null!));
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleSphere_ReturnsRequestedNumberOfPoints(int count, float jitter)
    {
        var result = _generator.SampleSphere(count, jitter);

        Assert.NotNull(result);
        Assert.Equal(count, result.Length);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleSphere_AllPointsLieOnUnitSphere(int count, float jitter)
    {
        var result = _generator.SampleSphere(count, jitter);

        Assert.All(result, point =>
        {
            float length = point.Length();
            Assert.True(
                MathF.Abs(length - 1f) < UnitSphereTolerance,
                $"Point ({point.X}, {point.Y}, {point.Z}) has length {length}, expected 1.");
        });
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleLatLong_ReturnsRequestedNumberOfPoints(int count, float jitter)
    {
        var result = _generator.SampleLatLong(count, jitter);

        Assert.NotNull(result);
        Assert.Equal(count, result.Length);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleLatLong_LongitudeIsNormalizedWithinPlusMinus360(int count, float jitter)
    {
        var result = _generator.SampleLatLong(count, jitter);

        Assert.All(result, point =>
        {
            Assert.True(point.Y > -360f && point.Y < 360f,
                $"Longitude {point.Y} is outside the (-360, 360) range.");
        });
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleLatLong_WithoutJitter_LatitudeWithinPolarRange(int count, float jitter)
    {
        _ = jitter;

        var result = _generator.SampleLatLong(count, jitter: 0f);

        Assert.All(result, point =>
        {
            Assert.InRange(point.X, -90f, 90f);
        });
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleSphere_WithoutJitter_IsDeterministic(int count, float jitter)
    {
        _ = jitter;

        var generatorA = new FibonacciSphereGenerator(new MersenneTwister(seed: 1));
        var generatorB = new FibonacciSphereGenerator(new MersenneTwister(seed: 2));

        var resultA = generatorA.SampleSphere(count);
        var resultB = generatorB.SampleSphere(count);

        Assert.Equal(resultA, resultB);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleSphere_WithSameSeedAndJitter_IsDeterministic(int count, float jitter)
    {
        _ = jitter;

        var generatorA = new FibonacciSphereGenerator(new MersenneTwister(seed: 123));
        var generatorB = new FibonacciSphereGenerator(new MersenneTwister(seed: 123));

        var resultA = generatorA.SampleSphere(count, jitter: 0.5f);
        var resultB = generatorB.SampleSphere(count, jitter: 0.5f);

        Assert.Equal(resultA, resultB);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleSphere_WithJitter_DiffersFromUnJittered(int count, float jitter)
    {
        _ = jitter;

        var generatorA = new FibonacciSphereGenerator(new MersenneTwister(seed: 7));
        var generatorB = new FibonacciSphereGenerator(new MersenneTwister(seed: 7));

        var unjittered = generatorA.SampleSphere(count, jitter: 0f);
        var jittered = generatorB.SampleSphere(count, jitter: 0.5f);

        Assert.NotEqual(unjittered, jittered);
    }

    [Theory]
    [ClassData(typeof(FibonacciSphereGeneratorPositiveData))]
    public void SampleSphere_PointsAreUnique(int count, float jitter)
    {
        _ = jitter;

        var result = _generator.SampleSphere(count);

        var unique = new HashSet<Vector3>(result);
        Assert.Equal(count, unique.Count);
    }

    [Fact]
    public void SampleSphere_CalledMultipleTimes_ReusesRandomBuffersWithoutError()
    {
        var result1 = _generator.SampleSphere(50, jitter: 0.5f);
        var result2 = _generator.SampleSphere(100, jitter: 0.5f);
        var result3 = _generator.SampleSphere(25, jitter: 0.5f);

        Assert.Equal(50, result1.Length);
        Assert.Equal(100, result2.Length);
        Assert.Equal(25, result3.Length);
    }

    [Fact]
    public void SampleSphere_WithZeroCount_ReturnsEmptyArray()
    {
        var result = _generator.SampleSphere(0);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void SampleLatLong_WithZeroCount_ReturnsEmptyArray()
    {
        var result = _generator.SampleLatLong(0);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
