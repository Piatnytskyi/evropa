namespace Evropa.Mathematics.Sampling.Regions;

using System.Numerics;

public readonly struct RectangleSamplingRegion : ISamplingRegion<Vector2>
{
    public Vector2 Min { get; }
    public Vector2 Max { get; }

    public RectangleSamplingRegion(Vector2 min, Vector2 max)
    {
        Min = min;
        Max = max;
    }

    public bool Contains(Vector2 point) =>
        point.X >= Min.X && point.X < Max.X &&
        point.Y >= Min.Y && point.Y < Max.Y;
}
