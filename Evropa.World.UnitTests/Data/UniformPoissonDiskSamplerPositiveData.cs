namespace Evropa.World.UnitTests.Data;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class UniformPoissonDiskSamplerPositiveData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        // center, radius, minimumDistance, pointsPerIteration
        new object[] { new Vector2(50, 50), 25.0f, 5.0f, 30 },
        new object[] { new Vector2(100, 100), 50.0f, 10.0f, 25 },
        new object[] { new Vector2(0, 0), 10.0f, 2.0f, 20 },
        new object[] { new Vector2(75, 75), 30.0f, 8.0f, 35 },
        new object[] { new Vector2(200, 150), 75.0f, 15.0f, 40 },
        new object[] { new Vector2(300, 200), 100.0f, 20.0f, 50 },
        new object[] { new Vector2(25, 25), 15.0f, 3.0f, 15 },
        new object[] { new Vector2(150, 100), 60.0f, 12.0f, 30 }
    };


    public IEnumerator<object[]> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
