namespace Evropa.Mathematics.Sampling.Regions;

using System.Numerics;

public readonly struct BoxSamplingRegion : ISamplingRegion<Vector3>
{
    public Vector3 Min { get; }
    public Vector3 Max { get; }

    public BoxSamplingRegion(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    public bool Contains(Vector3 point) =>
        point.X >= Min.X && point.X < Max.X &&
        point.Y >= Min.Y && point.Y < Max.Y &&
        point.Z >= Min.Z && point.Z < Max.Z;
}
