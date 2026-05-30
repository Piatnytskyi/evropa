namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Abstractions;
using MathNet.Numerics.Random;

// Based on "Fast Poisson Disk Sampling in Arbitrary Dimensions" and implementations from Red Blob Games.
// Algorithm from http://web.archive.org/web/20120421191837/http://www.cgafaq.info/wiki/Evenly_distributed_points_on_sphere
public class FibonacciSphereGenerator : ISphereSampler
{
    private readonly RandomSource _randomSource;

    private float[] _randomLat = Array.Empty<float>();
    private float[] _randomLon = Array.Empty<float>();

    public FibonacciSphereGenerator(RandomSource randomSource)
    {
        _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
    }

    public Vector3[] SampleSphere(int count, float jitter = 0f)
    {
        var latLongPoints = SampleLatLong(count, jitter);
        var result = new Vector3[count];

        for (int i = 0; i < latLongPoints.Length; i++)
        {
            result[i] = LatLongToCartesian(latLongPoints[i].X, latLongPoints[i].Y);
        }

        return result;
    }

    public Vector2[] SampleLatLong(int count, float jitter = 0f)
    {
        var result = new Vector2[count];

        float s = 3.6f / MathF.Sqrt(count);
        float dlong = MathF.PI * (3f - MathF.Sqrt(5f));
        float dz = 2.0f / count;

        float longitude = 0f;
        float z = 1f - dz / 2f;

        if (jitter > 0f)
        {
            EnsureRandomValues(count);
        }

        for (int k = 0; k < count; k++)
        {
            float r = MathF.Sqrt(1f - z * z);
            float latDeg = MathF.Asin(z) * 180f / MathF.PI;
            float lonDeg = longitude * 180f / MathF.PI;

            if (jitter > 0f)
            {
                float latJitter = latDeg - MathF.Asin(Math.Max(-1f, z - dz * 2f * MathF.PI * r / s)) * 180f / MathF.PI;
                latDeg += jitter * _randomLat[k] * latJitter;
                lonDeg += jitter * _randomLon[k] * (s / r * 180f / MathF.PI);
            }

            result[k] = new Vector2(latDeg, lonDeg % 360f);

            longitude += dlong;
            z -= dz;
        }

        return result;
    }

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

    private void EnsureRandomValues(int count)
    {
        if (_randomLat.Length < count)
        {
            int oldLen = _randomLat.Length;
            Array.Resize(ref _randomLat, count);
            for (int i = oldLen; i < count; i++)
            {
                _randomLat[i] = (float)(_randomSource.NextDouble() - _randomSource.NextDouble());
            }
        }
        if (_randomLon.Length < count)
        {
            int oldLen = _randomLon.Length;
            Array.Resize(ref _randomLon, count);
            for (int i = oldLen; i < count; i++)
            {
                _randomLon[i] = (float)(_randomSource.NextDouble() - _randomSource.NextDouble());
            }
        }
    }
}
