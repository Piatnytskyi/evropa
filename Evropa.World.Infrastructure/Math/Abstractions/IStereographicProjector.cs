namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Numerics;

public interface IStereographicProjector
{
    Vector2 ProjectToPlane(Vector3 spherePoint);

    Vector2[] ProjectToPlane(Vector3[] spherePoints);

    Vector3 ProjectToSphere(Vector2 planePoint);

    Vector3[] ProjectToSphere(Vector2[] planePoints);
}
