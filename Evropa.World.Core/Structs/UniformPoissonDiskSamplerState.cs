namespace Evropa.World.Core.Structs;

using System.Numerics;

public struct UniformPoissonDiskSamplerState
{
    public Vector2?[,] Grid;
    public List<Vector2> ActivePoints, Points;
}
