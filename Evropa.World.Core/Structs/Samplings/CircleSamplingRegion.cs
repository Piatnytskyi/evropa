namespace Evropa.World.Core.Structs.Samplings;

using System.Numerics;

public readonly struct CircleSamplingRegion : ISamplingRegion
{
    public Vector2 Center { get; }
    public float Radius { get; }
    public float MinimumDistance { get; }
    public int PointsPerIteration { get; }
    
    public Vector2 TopLeft => Center - new Vector2(Radius);
    public Vector2 LowerRight => Center + new Vector2(Radius);

    public CircleSamplingRegion(Vector2 center, float radius, float minimumDistance, int pointsPerIteration = 30)
    {
        Center = center;
        Radius = radius;
        MinimumDistance = minimumDistance;
        PointsPerIteration = pointsPerIteration;
    }

    public bool Contains(Vector2 point) => Vector2.Distance(Center, point) <= Radius;
}
