namespace Evropa.World.Core.Structs;

using System.Numerics;

public readonly struct CircleSamplingRegion
{
    public Vector2 Center { get; }
    public float Radius { get; }
    public float MinimumDistance { get; }
    public int PointsPerIteration { get; }

    public CircleSamplingRegion(Vector2 center, float radius, float minimumDistance, int pointsPerIteration = 30)
    {
        Center = center;
        Radius = radius;
        MinimumDistance = minimumDistance;
        PointsPerIteration = pointsPerIteration;
    }

    public static implicit operator SamplingRegion(CircleSamplingRegion circle) =>
        new(circle.Center - new Vector2(circle.Radius),
            circle.Center + new Vector2(circle.Radius),
            circle.Radius,
            circle.MinimumDistance,
            circle.PointsPerIteration);
}
