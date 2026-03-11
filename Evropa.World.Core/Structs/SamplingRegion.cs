namespace Evropa.World.Core.Structs;

using System.Numerics;

public readonly struct SamplingRegion
{
    public Vector2 TopLeft { get; }
    public Vector2 LowerRight { get; }
    public float? RejectionDistance { get; }
    public float MinimumDistance { get; }
    public int PointsPerIteration { get; }

    public SamplingRegion(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration = 30)
    {
        TopLeft = topLeft;
        LowerRight = lowerRight;
        RejectionDistance = rejectionDistance;
        MinimumDistance = minimumDistance;
        PointsPerIteration = pointsPerIteration;
    }
}
