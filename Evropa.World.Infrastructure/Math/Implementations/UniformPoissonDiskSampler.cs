namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
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
    private const int InitialCapacity = 64;

    private readonly RandomSource _randomSource;

    public UniformPoissonDiskSampler(RandomSource randomSource)
    {
        _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
    }

    public Vector2[] Sample(SamplingRegion region)
    {
        return Sample(region.TopLeft, region.LowerRight, region.RejectionDistance, region.MinimumDistance, region.PointsPerIteration);
    }

    private Vector2[] Sample(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration)
    {
        var settings = new Settings
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

        var state = new State
        {
            Grid = new Vector2?[settings.GridWidth, settings.GridHeight],
            ActivePoints = new Vector2[InitialCapacity],
            ActivePointsCount = 0,
            Points = new Vector2[InitialCapacity],
            PointsCount = 0
        };

        AddFirstPoint(ref settings, ref state);

        while (state.ActivePointsCount != 0)
        {
            var listIndex = _randomSource.Next(state.ActivePointsCount);

            var point = state.ActivePoints[listIndex];
            var found = false;

            for (var k = 0; k < pointsPerIteration; k++)
                found |= AddNextPoint(point, ref settings, ref state);

            if (!found)
            {
                state.ActivePoints[listIndex] = state.ActivePoints[--state.ActivePointsCount];
            }
        }

        if (state.PointsCount == state.Points.Length)
            return state.Points;

        var result = new Vector2[state.PointsCount];
        Array.Copy(state.Points, result, state.PointsCount);
        return result;
    }

    private static void Append(ref Vector2[] array, ref int count, Vector2 value)
    {
        if (count == array.Length)
            Array.Resize(ref array, array.Length * 2);
        array[count++] = value;
    }

    private void AddFirstPoint(ref Settings settings, ref State state)
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

            Append(ref state.ActivePoints, ref state.ActivePointsCount, p);
            Append(ref state.Points, ref state.PointsCount, p);
        } 
    }

    private bool AddNextPoint(Vector2 point, ref Settings settings, ref State state)
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
                Append(ref state.ActivePoints, ref state.ActivePointsCount, q);
                Append(ref state.Points, ref state.PointsCount, q);
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

    private struct Settings
    {
        public Vector2 TopLeft, LowerRight, Center;
        public Vector2 Dimensions;
        public float? RejectionSqDistance;
        public float MinimumDistance;
        public float CellSize;
        public int GridWidth, GridHeight;
    }

    private struct State
    {
        public Vector2?[,] Grid;
        public Vector2[] ActivePoints;
        public int ActivePointsCount;
        public Vector2[] Points;
        public int PointsCount;
    }
}

