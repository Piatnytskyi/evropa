namespace Evropa.World.UnitTests.Data;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class DelaunayTriangulatorPositiveData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        // Simple triangle.
        new object[]
        {
            new[]
            {
                new Vector2(0f, 0f),
                new Vector2(10f, 0f),
                new Vector2(5f, 8f),
            },
        },
        // Unit square (4 points → 2 triangles).
        new object[]
        {
            new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
            },
        },
        // Pentagon-ish.
        new object[]
        {
            new[]
            {
                new Vector2(0f, 0f),
                new Vector2(4f, 0f),
                new Vector2(5f, 3f),
                new Vector2(2f, 5f),
                new Vector2(-1f, 3f),
            },
        },
        // Grid 3x3.
        new object[]
        {
            new[]
            {
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(2f, 0f),
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(2f, 1f),
                new Vector2(0f, 2f), new Vector2(1f, 2f), new Vector2(2f, 2f),
            },
        },
        // Random-ish scattered cloud.
        new object[]
        {
            new[]
            {
                new Vector2(1.2f, 3.4f),
                new Vector2(4.7f, 1.1f),
                new Vector2(2.5f, 5.9f),
                new Vector2(6.8f, 4.2f),
                new Vector2(3.1f, 2.7f),
                new Vector2(5.5f, 6.3f),
                new Vector2(0.4f, 1.8f),
                new Vector2(7.0f, 0.5f),
                new Vector2(8.1f, 3.6f),
                new Vector2(4.0f, 7.2f),
                new Vector2(2.0f, 0.3f),
                new Vector2(6.0f, 2.5f),
            },
        },
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
