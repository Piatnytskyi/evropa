using System.Runtime.CompilerServices;

namespace Evropa.World.Core.Structs.Grids;

public readonly struct Cell<TVertices> : IEquatable<Cell<TVertices>>, IGrid where TVertices : ITuple
{
    public readonly TVertices Vertices;

    public Cell(TVertices vertices)
    {
        Vertices = vertices;
    }

    public bool Equals(Cell<TVertices> other) => Vertices.Equals(other.Vertices);
    public override bool Equals(object? obj) => obj is Cell<TVertices> c && Equals(c);
    public override int GetHashCode() => Vertices.GetHashCode();
}
