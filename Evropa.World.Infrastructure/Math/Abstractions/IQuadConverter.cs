namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Numerics;

public interface IQuadConverter
{
    (Vector2, Vector2, Vector2, Vector2)[] Convert((Vector2, Vector2, Vector2)[] triangles);
}
