namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Collections.Generic;
using System.Numerics;
using Evropa.Core.Constants;
using Evropa.World.Core.Structs;
using Evropa.World.Infrastructure.Math.Abstractions;
using MathNet.Numerics.Random;

// Adapated from java source by Herman Tulleken
// http://www.luma.co.za/labs/2008/02/27/poisson-disk-sampling/

// The algorithm is from the "Fast Poisson Disk Sampling in Arbitrary Dimensions" paper by Robert Bridson
// http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf

public class UniformPoissonDiskSampler : IDiskSampler
{
    public const int DefaultPointsPerIteration = 30;

    private readonly RandomSource _randomSource;

    public UniformPoissonDiskSampler(RandomSource randomSource)
    {
        _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
    }

    public List<Vector2> SampleCircle(Vector2 center, float radius, float minimumDistance, int pointsPerIteration = DefaultPointsPerIteration)
    {
        return Sample(center - new Vector2(radius), center + new Vector2(radius), radius, minimumDistance, pointsPerIteration);
    }

    public List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, int pointsPerIteration = DefaultPointsPerIteration)
    {
        return Sample(topLeft, lowerRight, null, minimumDistance, pointsPerIteration);
    }

    private List<Vector2> Sample(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration)
    {
        var settings = new UniformPoissonDiskSamplerSettings
        {
            TopLeft = topLeft, LowerRight = lowerRight,
            Dimensions = lowerRight - topLeft,
            Center = (topLeft + lowerRight) / 2,
            CellSize = minimumDistance / MathConstants.SquareRootTwo,
            MinimumDistance = minimumDistance,
            RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance
        };
        settings.GridWidth = (int) (settings.Dimensions.X / settings.CellSize) + 1;
        settings.GridHeight = (int) (settings.Dimensions.Y / settings.CellSize) + 1;

        var state = new UniformPoissonDiskSamplerState
        {
            Grid = new Vector2?[settings.GridWidth, settings.GridHeight],
            ActivePoints = new List<Vector2>(),
            Points = new List<Vector2>()
        };

        AddFirstPoint(ref settings, ref state);

        while (state.ActivePoints.Count != 0)
        {
            var listIndex = _randomSource.Next(state.ActivePoints.Count);

            var point = state.ActivePoints[listIndex];
            var found = false;

            for (var k = 0; k < pointsPerIteration; k++)
                found |= AddNextPoint(point, ref settings, ref state);

            if (!found)
                state.ActivePoints.RemoveAt(listIndex);
        }

        return state.Points;
    }

    private void AddFirstPoint(ref UniformPoissonDiskSamplerSettings settings, ref UniformPoissonDiskSamplerState state)
    {
        var added = false;
        while (!added)
        {
            var d = _randomSource.NextDouble();
            var xr = settings.TopLeft.X + settings.Dimensions.X * d;

            d = _randomSource.NextDouble();
            var yr = settings.TopLeft.Y + settings.Dimensions.Y * d;

            var p = new Vector2((float) xr, (float) yr);
            if (settings.RejectionSqDistance != null && Vector2.DistanceSquared(settings.Center, p) > settings.RejectionSqDistance)
                continue;
            added = true;

            var index = Denormalize(p, settings.TopLeft, settings.CellSize);

            state.Grid[(int) index.X, (int) index.Y] = p;

            state.ActivePoints.Add(p);
            state.Points.Add(p);
        } 
    }

    private bool AddNextPoint(Vector2 point, ref UniformPoissonDiskSamplerSettings settings, ref UniformPoissonDiskSamplerState state)
    {
        var found = false;
        var q = GenerateRandomAround(point, settings.MinimumDistance);

        if (q.X >= settings.TopLeft.X && q.X < settings.LowerRight.X && 
            q.Y > settings.TopLeft.Y && q.Y < settings.LowerRight.Y &&
            (settings.RejectionSqDistance == null || Vector2.DistanceSquared(settings.Center, q) <= settings.RejectionSqDistance))
        {
            var qIndex = Denormalize(q, settings.TopLeft, settings.CellSize);
            var tooClose = false;

            for (var i = (int)Math.Max(0, qIndex.X - 2); i < Math.Min(settings.GridWidth, qIndex.X + 3) && !tooClose; i++)
                for (var j = (int)Math.Max(0, qIndex.Y - 2); j < Math.Min(settings.GridHeight, qIndex.Y + 3) && !tooClose; j++)
                    if (state.Grid[i, j].HasValue && Vector2.Distance(state.Grid[i, j]!.Value, q) < settings.MinimumDistance)
                        tooClose = true;

            if (!tooClose)
            {
                found = true;
                state.ActivePoints.Add(q);
                state.Points.Add(q);
                state.Grid[(int)qIndex.X, (int)qIndex.Y] = q;
            }
        }
        return found;
    }

    private Vector2 GenerateRandomAround(Vector2 center, float minimumDistance)
    {
        var d = _randomSource.NextDouble();
        var radius = minimumDistance + minimumDistance * d;

        d = _randomSource.NextDouble();
        var angle = MathConstants.TwoPi * d;

        var newX = radius * Math.Sin(angle);
        var newY = radius * Math.Cos(angle);

        return new Vector2((float) (center.X + newX), (float) (center.Y + newY));
    }

    static Vector2 Denormalize(Vector2 point, Vector2 origin, double cellSize)
    {
        return new Vector2((int) ((point.X - origin.X) / cellSize), (int) ((point.Y - origin.Y) / cellSize));
    }
}

