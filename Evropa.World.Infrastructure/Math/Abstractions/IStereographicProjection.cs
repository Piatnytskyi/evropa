namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Collections.Generic;
using System.Numerics;

/// <summary>
/// Interface for stereographic projection between a sphere and an infinite plane.
/// </summary>
public interface IStereographicProjection
{
    /// <summary>
    /// Projects a point from a unit sphere onto an infinite plane using stereographic projection.
    /// The projection is from the south pole (0, 0, -1) onto the z=0 plane.
    /// </summary>
    /// <param name="spherePoint">A point on the unit sphere (x, y, z).</param>
    /// <returns>The projected point on the plane (X, Y).</returns>
    Vector2 ProjectToPlane(Vector3 spherePoint);

    /// <summary>
    /// Projects multiple points from a unit sphere onto an infinite plane.
    /// </summary>
    /// <param name="spherePoints">Points on the unit sphere.</param>
    /// <returns>The projected points on the plane.</returns>
    List<Vector2> ProjectToPlane(IEnumerable<Vector3> spherePoints);

    /// <summary>
    /// Projects a point from the plane back onto the unit sphere (inverse stereographic projection).
    /// </summary>
    /// <param name="planePoint">A point on the plane (X, Y).</param>
    /// <returns>The corresponding point on the unit sphere (x, y, z).</returns>
    Vector3 ProjectToSphere(Vector2 planePoint);

    /// <summary>
    /// Projects multiple points from the plane back onto the unit sphere.
    /// </summary>
    /// <param name="planePoints">Points on the plane.</param>
    /// <returns>The corresponding points on the unit sphere.</returns>
    List<Vector3> ProjectToSphere(IEnumerable<Vector2> planePoints);
}
