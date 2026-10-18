namespace Evropa.Mathematics.UnitTests.Data;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class UniformPoissonVolumeSamplingStrategyPositiveData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        // center, radius, minimumDistance, pointsPerIteration
        new object[] { new Vector3(50, 50, 50), 25.0f, 8.0f, 30 },
        new object[] { new Vector3(0, 0, 0), 15.0f, 5.0f, 25 },
        new object[] { new Vector3(100, 100, 100), 40.0f, 12.0f, 30 },
        new object[] { new Vector3(25, 25, 25), 20.0f, 6.0f, 20 }
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
