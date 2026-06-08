namespace Evropa.World.Core.Structs.Samplings;

using System.Numerics;

public readonly struct RectangleSamplingRegion : ISamplingRegion
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

    public bool Contains(Vector2 point) => 
        point.X >= TopLeft.X && point.X < LowerRight.X && 
        point.Y >= TopLeft.Y && point.Y < LowerRight.Y;
}
