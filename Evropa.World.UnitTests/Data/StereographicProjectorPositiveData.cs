namespace Evropa.World.UnitTests.Data;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class StereographicProjectorPositiveData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        // spherePoint (unit length, Z != -1), expectedPlanePoint
        new object[] { new Vector3(0f, 0f, 1f), new Vector2(0f, 0f) },
        new object[] { new Vector3(1f, 0f, 0f), new Vector2(1f, 0f) },
        new object[] { new Vector3(0f, 1f, 0f), new Vector2(0f, 1f) },
        new object[] { new Vector3(-1f, 0f, 0f), new Vector2(-1f, 0f) },
        new object[] { new Vector3(0f, -1f, 0f), new Vector2(0f, -1f) },
        new object[] { new Vector3(3f / 5f, 0f, 4f / 5f), new Vector2(1f / 3f, 0f) },
        new object[] { new Vector3(0f, 3f / 5f, 4f / 5f), new Vector2(0f, 1f / 3f) },
        new object[] { new Vector3(-3f / 5f, 0f, 4f / 5f), new Vector2(-1f / 3f, 0f) },
        new object[] { new Vector3(1f / 3f, 2f / 3f, 2f / 3f), new Vector2(1f / 5f, 2f / 5f) },
        new object[] { new Vector3(2f / 3f, -1f / 3f, 2f / 3f), new Vector2(2f / 5f, -1f / 5f) },
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
