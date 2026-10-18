namespace Evropa.World.Application.Planets;

using System;
using System.Collections.Generic;
using System.Numerics;
using Evropa.Mathematics.Projection;
using Evropa.Mathematics.Sampling;
using Evropa.Mathematics.Smoothing;
using Evropa.Mathematics.Tessellation;
using Evropa.Mathematics.Triangulation;

public class PlanetSurfaceGenerator : IPlanetSurfaceGenerator
{
    private readonly ISampler<(int Count, float Jitter), Vector3> _sphereSampler;
    private readonly IStereographicProjector _projector;
    private readonly ITriangulator _triangulator;
    private readonly IQuadConverter _quadConverter;
    private readonly IFaceSmoother _smoother;

    public PlanetSurfaceGenerator(
        ISampler<(int Count, float Jitter), Vector3> sphereSampler,
        IStereographicProjector projector,
        ITriangulator triangulator,
        IQuadConverter quadConverter,
        IFaceSmoother smoother)
    {
        _sphereSampler = sphereSampler ?? throw new ArgumentNullException(nameof(sphereSampler));
        _projector = projector ?? throw new ArgumentNullException(nameof(projector));
        _triangulator = triangulator ?? throw new ArgumentNullException(nameof(triangulator));
        _quadConverter = quadConverter ?? throw new ArgumentNullException(nameof(quadConverter));
        _smoother = smoother ?? throw new ArgumentNullException(nameof(smoother));
    }

    public Vector3[][] Generate(int pointCount, float jitter)
    {
        var spherePoints = _sphereSampler.Sample((pointCount, jitter));
        var planePoints = _projector.ProjectToPlane(spherePoints);

        var triangles = _triangulator.Triangulate(planePoints);
        var quads = _quadConverter.Convert(triangles);

        var planeFaces = new Vector2[quads.Length][];
        for (int i = 0; i < quads.Length; i++)
        {
            planeFaces[i] = new[] { quads[i].Item1, quads[i].Item2, quads[i].Item3, quads[i].Item4 };
        }
        planeFaces = _smoother.Smooth(planeFaces);

        var planeToSphere = new Dictionary<Vector2, Vector3>();
        for (int i = 0; i < planePoints.Length; i++)
        {
            planeToSphere.TryAdd(planePoints[i], spherePoints[i]);
        }

        var sphereFaces = new Vector3[planeFaces.Length][];
        for (int i = 0; i < planeFaces.Length; i++)
        {
            var planeFace = planeFaces[i];
            var sphereFace = new Vector3[planeFace.Length];
            for (int j = 0; j < planeFace.Length; j++)
            {
                sphereFace[j] = planeToSphere.TryGetValue(planeFace[j], out var spherePoint)
                    ? spherePoint
                    : _projector.ProjectToSphere(planeFace[j]);
            }
            sphereFaces[i] = sphereFace;
        }

        return sphereFaces;
    }
}
