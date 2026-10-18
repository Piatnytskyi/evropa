namespace Evropa.Mathematics.Sampling.Regions;

using System.Numerics;

public readonly struct CircleSamplingRegion : ISamplingRegion<Vector2>
{
    public Vector2 Center { get; }
    public float Radius { get; }

    public Vector2 Min => Center - new Vector2(Radius);
    public Vector2 Max => Center + new Vector2(Radius);

    public CircleSamplingRegion(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public bool Contains(Vector2 point) => Vector2.Distance(Center, point) <= Radius;
}
