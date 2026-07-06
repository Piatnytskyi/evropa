namespace Evropa.World.Core.Structs.Samplings;

using System;
using System.Numerics;

public readonly struct PolygonSamplingRegion : ISamplingRegion
{
    public Vector2[] Vertices { get; }
    public Vector2 TopLeft { get; }
    public Vector2 LowerRight { get; }
    public float MinimumDistance { get; }
    public int PointsPerIteration { get; }

    public PolygonSamplingRegion(Vector2[] vertices, float minimumDistance, int pointsPerIteration = 30)
    {
        if (vertices is null || vertices.Length < 3)
            throw new ArgumentException("Polygon must have at least 3 vertices.", nameof(vertices));

        Vertices = vertices;
        MinimumDistance = minimumDistance;
        PointsPerIteration = pointsPerIteration;

        var topLeft = vertices[0];
        var lowerRight = vertices[0];
        for (int i = 1; i < vertices.Length; i++)
        {
            topLeft = Vector2.Min(topLeft, vertices[i]);
            lowerRight = Vector2.Max(lowerRight, vertices[i]);
        }

        TopLeft = topLeft;
        LowerRight = lowerRight;
    }

    public bool Contains(Vector2 point)
    {
        var inside = false;
        for (int i = 0, j = Vertices.Length - 1; i < Vertices.Length; j = i++)
        {
            var vi = Vertices[i];
            var vj = Vertices[j];
            if ((vi.Y > point.Y) != (vj.Y > point.Y) &&
                point.X < (vj.X - vi.X) * (point.Y - vi.Y) / (vj.Y - vi.Y) + vi.X)
            {
                inside = !inside;
            }
        }

        return inside;
    }
}
