namespace Evropa.World.UnitTests.Data;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class QuadConverterPositiveData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        // Single isolated triangle (no merge possible → subdivided into 3 quads).
        new object[]
        {
            new (Vector2, Vector2, Vector2)[]
            {
                (new Vector2(0f, 0f), new Vector2(10f, 0f), new Vector2(5f, 8f)),
            },
        },
        // Two triangles sharing an edge → merged into 1 quad → 4 output quads.
        new object[]
        {
            new (Vector2, Vector2, Vector2)[]
            {
                (new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f)),
                (new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f)),
            },
        },
        // Two disjoint triangles (no shared edge → both subdivided → 6 output quads).
        new object[]
        {
            new (Vector2, Vector2, Vector2)[]
            {
                (new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 1f)),
                (new Vector2(10f, 10f), new Vector2(11f, 10f), new Vector2(10.5f, 11f)),
            },
        },
        // Fan triangulation of a pentagon (4 triangles from a shared apex).
        new object[]
        {
            new (Vector2, Vector2, Vector2)[]
            {
                (new Vector2(0f, 0f), new Vector2(4f, 0f), new Vector2(5f, 3f)),
                (new Vector2(0f, 0f), new Vector2(5f, 3f), new Vector2(2f, 5f)),
                (new Vector2(0f, 0f), new Vector2(2f, 5f), new Vector2(-1f, 3f)),
            },
        },
        // Triangulated unit square split into 4 triangles meeting at the center.
        new object[]
        {
            new (Vector2, Vector2, Vector2)[]
            {
                (new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0.5f)),
                (new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f)),
                (new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(0.5f, 0.5f)),
                (new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0.5f, 0.5f)),
            },
        },
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
