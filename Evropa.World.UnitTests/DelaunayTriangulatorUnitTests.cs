namespace Evropa.World.UnitTests;

using System;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Implementations;
using Evropa.World.UnitTests.Data;

public class DelaunayTriangulatorUnitTests
{
    private const float Tolerance = 1e-3f;

    private readonly DelaunayTriangulator _triangulator = new DelaunayTriangulator();

    [Theory]
    [ClassData(typeof(DelaunayTriangulatorNegativeData))]
    public void Triangulate_WithInvalidInput_ThrowsExpectedException(Vector2[] points, Type expectedExceptionType)
    {
        var exception = Assert.ThrowsAny<Exception>(() => _triangulator.Triangulate(points));

        Assert.IsAssignableFrom(expectedExceptionType, exception);
    }

    [Theory]
    [ClassData(typeof(DelaunayTriangulatorPositiveData))]
    public void Triangulate_ReturnsAtLeastOneTriangle(Vector2[] points)
    {
        var result = _triangulator.Triangulate(points);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Theory]
    [ClassData(typeof(DelaunayTriangulatorPositiveData))]
    public void Triangulate_TriangleCountIsWithinDelaunayBound(Vector2[] points)
    {
        var result = _triangulator.Triangulate(points);

        // For n >= 3 points, a Delaunay triangulation has at most 2n - 5 triangles.
        var maxTriangles = 2 * points.Length - 5;
        Assert.InRange(result.Length, 1, maxTriangles);
    }

    [Theory]
    [ClassData(typeof(DelaunayTriangulatorPositiveData))]
    public void Triangulate_AllTriangleVerticesComeFromInput(Vector2[] points)
    {
        var input = new HashSet<Vector2>(points);

        var result = _triangulator.Triangulate(points);

        Assert.All(result, triangle =>
        {
            Assert.Contains(triangle.Item1, input);
            Assert.Contains(triangle.Item2, input);
            Assert.Contains(triangle.Item3, input);
        });
    }

    [Theory]
    [ClassData(typeof(DelaunayTriangulatorPositiveData))]
    public void Triangulate_TrianglesAreNonDegenerate(Vector2[] points)
    {
        var result = _triangulator.Triangulate(points);

        Assert.All(result, triangle =>
        {
            var area = MathF.Abs(SignedArea(triangle.Item1, triangle.Item2, triangle.Item3));
            Assert.True(area > Tolerance,
                $"Triangle ({triangle.Item1}, {triangle.Item2}, {triangle.Item3}) is degenerate (area = {area}).");
        });
    }

    [Theory]
    [ClassData(typeof(DelaunayTriangulatorPositiveData))]
    public void Triangulate_SatisfiesDelaunayCircumcircleProperty(Vector2[] points)
    {
        var result = _triangulator.Triangulate(points);

        foreach (var (a, b, c) in result)
        {
            var (center, radiusSquared) = Circumcircle(a, b, c);

            foreach (var p in points)
            {
                if (p.Equals(a) || p.Equals(b) || p.Equals(c))
                {
                    continue;
                }

                var distanceSquared = Vector2.DistanceSquared(center, p);
                Assert.True(distanceSquared >= radiusSquared - Tolerance,
                    $"Point {p} lies strictly inside the circumcircle of triangle ({a}, {b}, {c}).");
            }
        }
    }

    private static float SignedArea(Vector2 a, Vector2 b, Vector2 c)
    {
        return 0.5f * ((b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y));
    }

    private static (Vector2 center, float radiusSquared) Circumcircle(Vector2 a, Vector2 b, Vector2 c)
    {
        float ax = a.X, ay = a.Y;
        float bx = b.X, by = b.Y;
        float cx = c.X, cy = c.Y;

        float d = 2f * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
        float ux = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
        float uy = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;

        var center = new Vector2(ux, uy);
        var radiusSquared = Vector2.DistanceSquared(center, a);
        return (center, radiusSquared);
    }
}
