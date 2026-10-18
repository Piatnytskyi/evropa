namespace Evropa.World.Application.Planets;

using System.Numerics;

public interface IPlanetSurfaceGenerator
{
    Vector3[][] Generate(int pointCount, float jitter);
}
