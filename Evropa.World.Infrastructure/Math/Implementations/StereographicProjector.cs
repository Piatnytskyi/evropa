namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Abstractions;

// See: https://en.wikipedia.org/wiki/Stereographic_projection
public class StereographicProjector : IStereographicProjector
{
    public Vector2 ProjectToPlane(Vector3 spherePoint)
    {
        float denominator = 1f + spherePoint.Z;

        if (Math.Abs(denominator) < float.Epsilon)
        {
            return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        }

        return new Vector2(
            spherePoint.X / denominator,
            spherePoint.Y / denominator
        );
    }

    public Vector2[] ProjectToPlane(Vector3[] spherePoints)
    {
        var result = new Vector2[spherePoints.Length];
        for (int i = 0; i < spherePoints.Length; i++)
        {
            result[i] = ProjectToPlane(spherePoints[i]);
        }
        return result;
    }

    public Vector3 ProjectToSphere(Vector2 planePoint)
    {
        float x = planePoint.X;
        float y = planePoint.Y;
        float rSquared = x * x + y * y;

        if (float.IsInfinity(rSquared) || float.IsNaN(rSquared))
        {
            return new Vector3(0f, 0f, -1f);
        }

        float denominator = 1f + rSquared;

        return new Vector3(
            2f * x / denominator,
            2f * y / denominator,
            (1f - rSquared) / denominator
        );
    }

    public Vector3[] ProjectToSphere(Vector2[] planePoints)
    {
        var result = new Vector3[planePoints.Length];
        for (int i = 0; i < planePoints.Length; i++)
        {
            result[i] = ProjectToSphere(planePoints[i]);
        }
        return result;
    }
}
