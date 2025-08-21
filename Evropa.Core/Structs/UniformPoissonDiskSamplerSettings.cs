namespace Evropa.Core.Structs;

using System.Numerics;

public struct UniformPoissonDiskSamplerSettings
{
    public Vector2 TopLeft, LowerRight, Center;
    public Vector2 Dimensions;
    public float? RejectionSqDistance;
    public float MinimumDistance;
    public float CellSize;
    public int GridWidth, GridHeight;
}
