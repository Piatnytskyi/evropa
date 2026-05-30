namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Numerics;

public interface ISphereSampler
{
    Vector3[] SampleSphere(int count, float jitter = 0f);

    Vector2[] SampleLatLong(int count, float jitter = 0f);
}
