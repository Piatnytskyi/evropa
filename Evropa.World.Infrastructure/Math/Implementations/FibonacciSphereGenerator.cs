namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Collections.Generic;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Abstractions;
using MathNet.Numerics.Random;

/// <summary>
/// Generates evenly distributed points on a sphere using Fibonacci spiral algorithm.
/// Based on "Fast Poisson Disk Sampling in Arbitrary Dimensions" and implementations from Red Blob Games.
/// </summary>
public class FibonacciSphereGenerator : ISphereSampler
{
    private readonly RandomSource _randomSource;
    
    // Cache for random jitter values to ensure consistent results when rotating
    private readonly List<float> _randomLat = new();
    private readonly List<float> _randomLon = new();

    public FibonacciSphereGenerator(RandomSource randomSource)
    {
        _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
    }

    /// <inheritdoc />
    public List<Vector3> SampleSphere(int count, float jitter = 0f)
    {
        var latLongPoints = SampleLatLong(count, jitter);
        var result = new List<Vector3>(count);

        foreach (var latLong in latLongPoints)
        {
            result.Add(LatLongToCartesian(latLong.X, latLong.Y));
        }

        return result;
    }

    /// <inheritdoc />
    public List<Vector2> SampleLatLong(int count, float jitter = 0f)
    {
        var result = new List<Vector2>(count);
        
        // Algorithm from http://web.archive.org/web/20120421191837/http://www.cgafaq.info/wiki/Evenly_distributed_points_on_sphere
        // Using the golden angle approach for even distribution
        float s = 3.6f / MathF.Sqrt(count);
        float dlong = MathF.PI * (3f - MathF.Sqrt(5f)); // Golden angle ~2.39996323 radians
        float dz = 2.0f / count;

        float longitude = 0f;
        float z = 1f - dz / 2f;

        for (int k = 0; k < count; k++)
        {
            float r = MathF.Sqrt(1f - z * z);
            float latDeg = MathF.Asin(z) * 180f / MathF.PI;
            float lonDeg = longitude * 180f / MathF.PI;

            // Apply jitter if requested
            if (jitter > 0f)
            {
                EnsureRandomValues(k);
                
                float latJitter = latDeg - MathF.Asin(Math.Max(-1f, z - dz * 2f * MathF.PI * r / s)) * 180f / MathF.PI;
                latDeg += jitter * _randomLat[k] * latJitter;
                lonDeg += jitter * _randomLon[k] * (s / r * 180f / MathF.PI);
            }

            result.Add(new Vector2(latDeg, lonDeg % 360f));

            longitude += dlong;
            z -= dz;
        }

        return result;
    }

    /// <summary>
    /// Converts latitude/longitude in degrees to Cartesian coordinates on a unit sphere.
    /// </summary>
    private static Vector3 LatLongToCartesian(float latDeg, float lonDeg)
    {
        float latRad = latDeg / 180f * MathF.PI;
        float lonRad = lonDeg / 180f * MathF.PI;

        return new Vector3(
            MathF.Cos(latRad) * MathF.Cos(lonRad),
            MathF.Cos(latRad) * MathF.Sin(lonRad),
            MathF.Sin(latRad)
        );
    }

    /// <summary>
    /// Ensures random jitter values are generated up to the given index.
    /// Values are cached to ensure consistent results across calls.
    /// </summary>
    private void EnsureRandomValues(int index)
    {
        while (_randomLat.Count <= index)
        {
            _randomLat.Add((float)(_randomSource.NextDouble() - _randomSource.NextDouble()));
        }
        while (_randomLon.Count <= index)
        {
            _randomLon.Add((float)(_randomSource.NextDouble() - _randomSource.NextDouble()));
        }
    }
}
