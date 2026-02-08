namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Collections.Generic;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Abstractions;

/// <summary>
/// Implements stereographic projection between a unit sphere and an infinite plane.
/// 
/// The projection maps points from the unit sphere onto the z=0 plane, using the south pole (0, 0, -1)
/// as the projection point. Points near the south pole map to infinity on the plane.
/// 
/// See: https://en.wikipedia.org/wiki/Stereographic_projection
/// </summary>
public class StereographicProjection : IStereographicProjection
{
    /// <inheritdoc />
    public Vector2 ProjectToPlane(Vector3 spherePoint)
    {
        // Stereographic projection formula from south pole:
        // For a point (x, y, z) on the unit sphere, the projection onto z=0 plane is:
        // X = x / (1 + z)
        // Y = y / (1 + z)
        // 
        // Note: We use (1 + z) for projection from south pole (0, 0, -1)
        // The original JS uses (1 - z) for projection from north pole
        
        float denominator = 1f + spherePoint.Z;
        
        // Handle the south pole case - it maps to infinity
        if (Math.Abs(denominator) < float.Epsilon)
        {
            return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        }

        return new Vector2(
            spherePoint.X / denominator,
            spherePoint.Y / denominator
        );
    }

    /// <inheritdoc />
    public List<Vector2> ProjectToPlane(IEnumerable<Vector3> spherePoints)
    {
        var result = new List<Vector2>();
        foreach (var point in spherePoints)
        {
            result.Add(ProjectToPlane(point));
        }
        return result;
    }

    /// <inheritdoc />
    public Vector3 ProjectToSphere(Vector2 planePoint)
    {
        // Inverse stereographic projection formula:
        // Given a point (X, Y) on the plane, the corresponding point on the sphere is:
        // 
        // Let r² = X² + Y²
        // x = 2X / (1 + r²)
        // y = 2Y / (1 + r²)
        // z = (1 - r²) / (1 + r²)
        //
        // This is the inverse of projection from south pole.
        
        float x = planePoint.X;
        float y = planePoint.Y;
        float rSquared = x * x + y * y;
        
        // Handle infinity case - maps back to south pole
        if (float.IsInfinity(rSquared) || float.IsNaN(rSquared))
        {
            return new Vector3(0f, 0f, -1f); // South pole
        }

        float denominator = 1f + rSquared;

        return new Vector3(
            2f * x / denominator,
            2f * y / denominator,
            (1f - rSquared) / denominator
        );
    }

    /// <inheritdoc />
    public List<Vector3> ProjectToSphere(IEnumerable<Vector2> planePoints)
    {
        var result = new List<Vector3>();
        foreach (var point in planePoints)
        {
            result.Add(ProjectToSphere(point));
        }
        return result;
    }
}
