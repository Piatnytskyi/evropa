namespace Evropa.Mathematics.Sampling.Regions;

using System;
using System.Numerics;

public readonly struct PolygonSamplingRegion : ISamplingRegion<Vector2>
{
    public Vector2[] Vertices { get; }
    public Vector2 Min { get; }
    public Vector2 Max { get; }

    public PolygonSamplingRegion(Vector2[] vertices)
    {
        if (vertices is null || vertices.Length < 3)
            throw new ArgumentException("Polygon must have at least 3 vertices.", nameof(vertices));

        Vertices = vertices;

        var min = vertices[0];
        var max = vertices[0];
        for (int i = 1; i < vertices.Length; i++)
        {
            min = Vector2.Min(min, vertices[i]);
            max = Vector2.Max(max, vertices[i]);
        }

        Min = min;
        Max = max;
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
