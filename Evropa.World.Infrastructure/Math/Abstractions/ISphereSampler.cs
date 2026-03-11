namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Collections.Generic;
using System.Numerics;

/// <summary>
/// Interface for generating evenly distributed points on a sphere.
/// </summary>
public interface ISphereSampler
{
    /// <summary>
    /// Generates N points on a unit sphere.
    /// </summary>
    /// <param name="count">Number of points to generate.</param>
    /// <param name="jitter">Amount of randomness to add to point positions (0 = no jitter, 1 = maximum jitter).</param>
    /// <returns>List of 3D points on the unit sphere.</returns>
    List<Vector3> SampleSphere(int count, float jitter = 0f);

    /// <summary>
    /// Generates N points on a unit sphere and returns them as latitude/longitude pairs.
    /// </summary>
    /// <param name="count">Number of points to generate.</param>
    /// <param name="jitter">Amount of randomness to add to point positions.</param>
    /// <returns>List of (latitude, longitude) pairs in degrees.</returns>
    List<Vector2> SampleLatLong(int count, float jitter = 0f);
}
