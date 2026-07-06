namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Numerics;

public interface IVoronoiDiagramCalculator
{
    Vector2[][] Calculate((Vector2, Vector2, Vector2)[] triangles);
}
