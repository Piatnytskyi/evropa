namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Collections.Generic;
using System.Numerics;

public interface IQuadConverter
{
    List<(Vector2, Vector2, Vector2, Vector2)> ConvertToQuads(List<(Vector2, Vector2, Vector2)> triangles);
}
