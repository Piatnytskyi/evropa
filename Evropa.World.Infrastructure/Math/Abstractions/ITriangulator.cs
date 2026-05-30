namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Numerics;

public interface ITriangulator
{
    (Vector2, Vector2, Vector2)[] Triangulate(Vector2[] points);
}
