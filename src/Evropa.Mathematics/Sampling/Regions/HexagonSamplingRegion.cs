namespace Evropa.Mathematics.Sampling.Regions;

using System;
using System.Numerics;

public readonly struct HexagonSamplingRegion : ISamplingRegion<Vector2>
{
    public Vector2 Center { get; }
    public float Radius { get; }
    public HexagonOrientation Orientation { get; }

    public Vector2 Min => Center - new Vector2(Radius);
    public Vector2 Max => Center + new Vector2(Radius);

    public HexagonSamplingRegion(Vector2 center, float radius, HexagonOrientation orientation = HexagonOrientation.PointyTop)
    {
        Center = center;
        Radius = radius;
        Orientation = orientation;
    }

    public bool Contains(Vector2 point)
    {
        var relative = point - Center;
        var distance = relative.Length();

        if (distance > Radius)
            return false;

        var angle = (float)Math.Atan2(relative.Y, relative.X);
        var apothem = Radius * (float)Math.Cos(Math.PI / 6);

        // Adjust starting angle based on orientation
        var angleOffset = Orientation == HexagonOrientation.FlatTop ? (float)(Math.PI / 6) : 0f;
        var normalizedAngle = angle - angleOffset;
        normalizedAngle = normalizedAngle % (float)(Math.PI / 3);
        if (normalizedAngle < 0)
            normalizedAngle += (float)(Math.PI / 3);

        var effectiveDistance = distance * (float)Math.Cos(normalizedAngle - (float)(Math.PI / 6));
        return effectiveDistance <= apothem;
    }
}
