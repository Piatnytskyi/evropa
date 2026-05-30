namespace Evropa.World.UnitTests;

using System;
using System.Collections.Generic;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Implementations;
using Evropa.World.UnitTests.Data;

public class StereographicProjectorUnitTests
{
    private const float Tolerance = 1e-4f;

    private readonly StereographicProjector _projector = new StereographicProjector();

    [Fact]
    public void ProjectToPlane_SouthPole_ReturnsPositiveInfinity()
    {
        var result = _projector.ProjectToPlane(new Vector3(0f, 0f, -1f));

        Assert.Equal(float.PositiveInfinity, result.X);
        Assert.Equal(float.PositiveInfinity, result.Y);
    }

    [Fact]
    public void ProjectToSphere_PositiveInfinity_ReturnsSouthPole()
    {
        var result = _projector.ProjectToSphere(new Vector2(float.PositiveInfinity, float.PositiveInfinity));

        Assert.Equal(new Vector3(0f, 0f, -1f), result);
    }

    [Fact]
    public void ProjectToSphere_NaN_ReturnsSouthPole()
    {
        var result = _projector.ProjectToSphere(new Vector2(float.NaN, float.NaN));

        Assert.Equal(new Vector3(0f, 0f, -1f), result);
    }

    [Theory]
    [ClassData(typeof(StereographicProjectorPositiveData))]
    public void ProjectToPlane_MapsSpherePointToExpectedPlanePoint(
        Vector3 spherePoint, Vector2 expectedPlanePoint)
    {
        var result = _projector.ProjectToPlane(spherePoint);

        AssertApproxEqual(expectedPlanePoint.X, result.X);
        AssertApproxEqual(expectedPlanePoint.Y, result.Y);
    }

    [Theory]
    [ClassData(typeof(StereographicProjectorPositiveData))]
    public void ProjectToSphere_MapsPlanePointToExpectedSpherePoint(
        Vector3 expectedSpherePoint, Vector2 planePoint)
    {
        var result = _projector.ProjectToSphere(planePoint);

        AssertApproxEqual(expectedSpherePoint.X, result.X);
        AssertApproxEqual(expectedSpherePoint.Y, result.Y);
        AssertApproxEqual(expectedSpherePoint.Z, result.Z);
    }

    [Theory]
    [ClassData(typeof(StereographicProjectorPositiveData))]
    public void ProjectToPlane_ThenProjectToSphere_ReturnsOriginalPoint(
        Vector3 spherePoint, Vector2 planePoint)
    {
        _ = planePoint;

        var recovered = _projector.ProjectToSphere(_projector.ProjectToPlane(spherePoint));

        AssertApproxEqual(spherePoint.X, recovered.X);
        AssertApproxEqual(spherePoint.Y, recovered.Y);
        AssertApproxEqual(spherePoint.Z, recovered.Z);
    }

    [Theory]
    [ClassData(typeof(StereographicProjectorPositiveData))]
    public void ProjectToSphere_ThenProjectToPlane_ReturnsOriginalPoint(
        Vector3 spherePoint, Vector2 planePoint)
    {
        _ = spherePoint;

        var recovered = _projector.ProjectToPlane(_projector.ProjectToSphere(planePoint));

        AssertApproxEqual(planePoint.X, recovered.X);
        AssertApproxEqual(planePoint.Y, recovered.Y);
    }

    [Theory]
    [ClassData(typeof(StereographicProjectorPositiveData))]
    public void ProjectToSphere_OutputHasUnitLength(Vector3 spherePoint, Vector2 planePoint)
    {
        _ = spherePoint;

        var result = _projector.ProjectToSphere(planePoint);

        AssertApproxEqual(1f, result.Length());
    }

    [Fact]
    public void ProjectToPlane_NullArray_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => _projector.ProjectToPlane((Vector3[])null!));
    }

    [Fact]
    public void ProjectToSphere_NullArray_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => _projector.ProjectToSphere((Vector2[])null!));
    }

    [Fact]
    public void ProjectToPlane_EmptyArray_ReturnsEmptyArray()
    {
        var result = _projector.ProjectToPlane(Array.Empty<Vector3>());

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ProjectToSphere_EmptyArray_ReturnsEmptyArray()
    {
        var result = _projector.ProjectToSphere(Array.Empty<Vector2>());

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private static void AssertApproxEqual(float expected, float actual)
    {
        Assert.True(
            MathF.Abs(expected - actual) < Tolerance,
            $"Expected {expected} ± {Tolerance}, got {actual}.");
    }
}
