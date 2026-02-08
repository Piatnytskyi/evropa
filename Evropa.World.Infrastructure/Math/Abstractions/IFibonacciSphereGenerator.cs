namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Collections.Generic;
using System.Numerics;

/// <summary>
/// Interface for generating evenly distributed points on a sphere using Fibonacci spiral.
/// </summary>
public interface IFibonacciSphereGenerator
{
    /// <summary>
    /// Generates N points on a unit sphere using Fibonacci spiral algorithm.
    /// </summary>
    /// <param name="count">Number of points to generate.</param>
    /// <param name="jitter">Amount of randomness to add to point positions (0 = no jitter, 1 = maximum jitter).</param>
    /// <returns>List of 3D points on the unit sphere.</returns>
    List<Vector3> GenerateSpherePoints(int count, float jitter = 0f);

    /// <summary>
    /// Generates N points on a unit sphere and returns them as latitude/longitude pairs.
    /// </summary>
    /// <param name="count">Number of points to generate.</param>
    /// <param name="jitter">Amount of randomness to add to point positions.</param>
    /// <returns>List of (latitude, longitude) pairs in degrees.</returns>
    List<Vector2> GenerateLatLongPoints(int count, float jitter = 0f);
}
