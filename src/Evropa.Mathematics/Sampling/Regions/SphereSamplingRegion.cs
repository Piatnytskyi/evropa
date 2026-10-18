namespace Evropa.Mathematics.Sampling.Regions;

using System.Numerics;

public readonly struct SphereSamplingRegion : ISamplingRegion<Vector3>
{
    public Vector3 Center { get; }
    public float Radius { get; }

    public Vector3 Min => Center - new Vector3(Radius);
    public Vector3 Max => Center + new Vector3(Radius);

    public SphereSamplingRegion(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public bool Contains(Vector3 point) => Vector3.Distance(Center, point) <= Radius;
}
