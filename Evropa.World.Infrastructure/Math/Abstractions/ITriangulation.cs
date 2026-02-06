namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Numerics;

/// <summary>
/// Interface for triangulation algorithms.
/// </summary>
public interface ITriangulation
{
    /// <summary>
    /// Computes triangulation for a set of 2D points.
    /// </summary>
    /// <param name="points">Array of 2D points to triangulate.</param>
    /// <returns>Array of triangles, where each triangle is represented by three Vector2 vertices in counter-clockwise order.</returns>
    (Vector2, Vector2, Vector2)[] Triangulate(Vector2[] points);
}
