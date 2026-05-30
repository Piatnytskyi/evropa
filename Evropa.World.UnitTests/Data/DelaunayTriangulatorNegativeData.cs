namespace Evropa.World.UnitTests.Data;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class DelaunayTriangulatorNegativeData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        // points, expectedExceptionType
        new object[] { Array.Empty<Vector2>(), typeof(ArgumentOutOfRangeException) },
        new object[] { new[] { new Vector2(0f, 0f) }, typeof(ArgumentOutOfRangeException) },
        new object[] { new[] { new Vector2(0f, 0f), new Vector2(1f, 1f) }, typeof(ArgumentOutOfRangeException) },
        // Fully collinear: no Delaunay triangulation exists.
        new object[]
        {
            new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(2f, 0f),
            },
            typeof(Exception),
        },
        new object[]
        {
            new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(2f, 2f),
                new Vector2(3f, 3f),
            },
            typeof(Exception),
        },
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
