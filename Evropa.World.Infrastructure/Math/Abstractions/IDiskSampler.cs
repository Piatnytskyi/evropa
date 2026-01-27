namespace Evropa.World.Infrastructure.Math.Abstractions;

using System.Collections.Generic;
using System.Numerics;

public interface IDiskSampler
{
    List<Vector2> SampleCircle(Vector2 center, float radius, float minimumDistance, int pointsPerIteration = 30);
    List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, int pointsPerIteration = 30);
}