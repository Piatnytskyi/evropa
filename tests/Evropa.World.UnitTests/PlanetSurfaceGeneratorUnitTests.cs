namespace Evropa.World.UnitTests;

using System;
using System.Numerics;
using Evropa.Mathematics.Projection;
using Evropa.Mathematics.Sampling;
using Evropa.Mathematics.Smoothing;
using Evropa.Mathematics.Tessellation;
using Evropa.Mathematics.Triangulation;
using Evropa.World.Application.Planets;
using MathNet.Numerics.Random;

public class PlanetSurfaceGeneratorUnitTests
{
    private const float UnitSphereTolerance = 1e-3f;

    [Fact]
    public void Constructor_WithNullDependency_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PlanetSurfaceGenerator(
            null!, new StereographicProjector(), new DelaunayTriangulator(), new QuadConverter(), new LaplacianSmoother()));
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public void Generate_ReturnsQuadFaces(int pointCount)
    {
        var faces = CreateGenerator().Generate(pointCount, 0f);

        Assert.NotEmpty(faces);
        Assert.All(faces, face => Assert.Equal(4, face.Length));
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public void Generate_AllVerticesLieOnUnitSphere(int pointCount)
    {
        var faces = CreateGenerator().Generate(pointCount, 0f);

        Assert.All(faces, face => Assert.All(face, vertex =>
        {
            float length = vertex.Length();
            Assert.True(
                MathF.Abs(length - 1f) < UnitSphereTolerance,
                $"Vertex ({vertex.X}, {vertex.Y}, {vertex.Z}) has length {length}, expected 1.");
        }));
    }

    private static PlanetSurfaceGenerator CreateGenerator() =>
        new PlanetSurfaceGenerator(
            new Sampler<(int Count, float Jitter), Vector3>(
                new FibonacciSphereSamplingStrategy(new MersenneTwister(seed: 42))),
            new StereographicProjector(),
            new DelaunayTriangulator(),
            new QuadConverter(),
            new LaplacianSmoother());
}
