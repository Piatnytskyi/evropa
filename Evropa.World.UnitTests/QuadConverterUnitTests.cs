namespace Evropa.World.UnitTests;

using System;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Implementations;
using Evropa.World.UnitTests.Data;

public class QuadConverterUnitTests
{
    private const float Tolerance = 1e-3f;

    private readonly QuadConverter _converter = new QuadConverter();

    [Fact]
    public void ConvertToQuads_WithEmptyInput_ReturnsEmptyArray()
    {
        var result = _converter.Convert(Array.Empty<(Vector2, Vector2, Vector2)>());

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ConvertToQuads_WithNullInput_Throws()
    {
        Assert.ThrowsAny<Exception>(() => _converter.Convert(null!));
    }

    [Theory]
    [ClassData(typeof(QuadConverterPositiveData))]
    public void ConvertToQuads_ReturnsNonEmptyResult((Vector2, Vector2, Vector2)[] triangles)
    {
        var result = _converter.Convert(triangles);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Theory]
    [ClassData(typeof(QuadConverterPositiveData))]
    public void ConvertToQuads_OutputCountIsWithinBounds((Vector2, Vector2, Vector2)[] triangles)
    {
        // Output count = 3 * remainingTriangles + 4 * mergedQuads, where
        // mergedQuads ∈ [0, triangles.Length / 2]. This yields count ∈ [2n, 3n]
        // and (3n - count) must be even (each merge replaces 2 triangles' 6 quads with 4).
        var n = triangles.Length;

        var result = _converter.Convert(triangles);

        Assert.InRange(result.Length, 2 * n, 3 * n);
        Assert.Equal(0, (3 * n - result.Length) % 2);
    }

    [Theory]
    [ClassData(typeof(QuadConverterPositiveData))]
    public void ConvertToQuads_AllQuadsHaveDistinctVertices((Vector2, Vector2, Vector2)[] triangles)
    {
        var result = _converter.Convert(triangles);

        Assert.All(result, quad =>
        {
            var vertices = new HashSet<Vector2> { quad.Item1, quad.Item2, quad.Item3, quad.Item4 };
            Assert.Equal(4, vertices.Count);
        });
    }

    [Theory]
    [ClassData(typeof(QuadConverterPositiveData))]
    public void ConvertToQuads_AllQuadsAreNonDegenerate((Vector2, Vector2, Vector2)[] triangles)
    {
        var result = _converter.Convert(triangles);

        Assert.All(result, quad =>
        {
            var area = MathF.Abs(QuadSignedArea(quad.Item1, quad.Item2, quad.Item3, quad.Item4));
            Assert.True(area > Tolerance,
                $"Quad ({quad.Item1}, {quad.Item2}, {quad.Item3}, {quad.Item4}) is degenerate (area = {area}).");
        });
    }

    [Theory]
    [ClassData(typeof(QuadConverterPositiveData))]
    public void ConvertToQuads_PreservesTotalArea((Vector2, Vector2, Vector2)[] triangles)
    {
        float inputArea = 0f;
        foreach (var (a, b, c) in triangles)
        {
            inputArea += MathF.Abs(TriangleSignedArea(a, b, c));
        }

        var result = _converter.Convert(triangles);

        float outputArea = 0f;
        foreach (var quad in result)
        {
            outputArea += MathF.Abs(QuadSignedArea(quad.Item1, quad.Item2, quad.Item3, quad.Item4));
        }

        Assert.True(MathF.Abs(inputArea - outputArea) < Tolerance,
            $"Total area not preserved: input = {inputArea}, output = {outputArea}.");
    }

    [Theory]
    [ClassData(typeof(QuadConverterPositiveData))]
    public void ConvertToQuads_AllOutputVerticesLieWithinInputBoundingBox((Vector2, Vector2, Vector2)[] triangles)
    {
        float minX = float.PositiveInfinity, minY = float.PositiveInfinity;
        float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;
        foreach (var (a, b, c) in triangles)
        {
            foreach (var v in new[] { a, b, c })
            {
                if (v.X < minX) minX = v.X;
                if (v.Y < minY) minY = v.Y;
                if (v.X > maxX) maxX = v.X;
                if (v.Y > maxY) maxY = v.Y;
            }
        }

        var result = _converter.Convert(triangles);

        Assert.All(result, quad =>
        {
            foreach (var v in new[] { quad.Item1, quad.Item2, quad.Item3, quad.Item4 })
            {
                Assert.InRange(v.X, minX - Tolerance, maxX + Tolerance);
                Assert.InRange(v.Y, minY - Tolerance, maxY + Tolerance);
            }
        });
    }

    private static float TriangleSignedArea(Vector2 a, Vector2 b, Vector2 c)
    {
        return 0.5f * ((b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y));
    }

    private static float QuadSignedArea(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        return 0.5f * (
            a.X * b.Y - b.X * a.Y +
            b.X * c.Y - c.X * b.Y +
            c.X * d.Y - d.X * c.Y +
            d.X * a.Y - a.X * d.Y);
    }
}
