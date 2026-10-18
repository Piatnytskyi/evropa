namespace Evropa.Mathematics.Smoothing;

using System.Numerics;

public interface IFaceSmoother
{
    Vector2[][] Smooth(Vector2[][] faces, int iterations = 10);
}
