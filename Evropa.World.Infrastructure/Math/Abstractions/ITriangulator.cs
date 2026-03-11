namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Collections.Generic;
using System.Numerics;

public interface ITriangulator
{
    List<(Vector2, Vector2, Vector2)> Triangulate(List<Vector2> points);
}
