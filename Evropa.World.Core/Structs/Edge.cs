using System.Numerics;

namespace Evropa.World.Core.Structs;

public readonly struct Edge : IEquatable<Edge>
{
    public readonly Vector2 V1;
    public readonly Vector2 V2;

    public Edge(Vector2 a, Vector2 b)
    {
        if (a.X < b.X || (a.X == b.X && a.Y <= b.Y))
        {
            V1 = a;
            V2 = b;
        }
        else
        {
            V1 = b;
            V2 = a;
        }
    }

    public bool Equals(Edge other) => V1.Equals(other.V1) && V2.Equals(other.V2);
    public override bool Equals(object? obj) => obj is Edge e && Equals(e);
    public override int GetHashCode() => HashCode.Combine(V1, V2);
}
