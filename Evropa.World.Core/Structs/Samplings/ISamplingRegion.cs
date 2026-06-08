namespace Evropa.World.Core.Structs.Samplings;

using System.Numerics;

public interface ISamplingRegion
{
    Vector2 TopLeft { get; }
    Vector2 LowerRight { get; }
    float MinimumDistance { get; }
    int PointsPerIteration { get; }
    bool Contains(Vector2 point);
}
