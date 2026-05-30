namespace Evropa.World.UnitTests.Data;

using System.Collections;
using System.Collections.Generic;

public class FibonacciSphereGeneratorPositiveData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        // count, jitter
        new object[] { 1, 0f },
        new object[] { 2, 0f },
        new object[] { 10, 0f },
        new object[] { 50, 0f },
        new object[] { 100, 0f },
        new object[] { 500, 0f },
        new object[] { 1000, 0f },
        new object[] { 10, 0.5f },
        new object[] { 100, 0.25f },
        new object[] { 500, 1.0f },
        new object[] { 1000, 0.75f },
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
