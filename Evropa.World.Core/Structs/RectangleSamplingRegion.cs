namespace Evropa.World.Core.Structs;

using System.Numerics;

public readonly struct RectangleSamplingRegion
{
    public Vector2 TopLeft { get; }
    public Vector2 LowerRight { get; }
    public float MinimumDistance { get; }
    public int PointsPerIteration { get; }

    public RectangleSamplingRegion(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, int pointsPerIteration = 30)
    {
        TopLeft = topLeft;
        LowerRight = lowerRight;
        MinimumDistance = minimumDistance;
        PointsPerIteration = pointsPerIteration;
    }

    public static implicit operator SamplingRegion(RectangleSamplingRegion rect) =>
        new(rect.TopLeft, rect.LowerRight, null, rect.MinimumDistance, rect.PointsPerIteration);
}
