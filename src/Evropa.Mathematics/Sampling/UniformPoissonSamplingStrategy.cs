namespace Evropa.Mathematics.Sampling;

using System;
using System.Numerics;
using Evropa.Mathematics.Sampling.Regions;
using MathNet.Numerics.Random;

// Adapted from java source by Herman Tulleken
// http://www.luma.co.za/labs/2008/02/27/poisson-disk-sampling/

// The algorithm is from the "Fast Poisson Disk Sampling in Arbitrary Dimensions" paper by Robert Bridson
// http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf

public class UniformPoissonSamplingStrategy<TRegion, TPoint>
    : ISamplingStrategy<(TRegion Region, float MinimumDistance, int PointsPerIteration), TPoint>
    where TRegion : ISamplingRegion<TPoint>
{
    private const int InitialCapacity = 64;

    private readonly RandomSource _randomSource;

    public UniformPoissonSamplingStrategy(RandomSource randomSource)
    {
        _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
    }

    public TPoint[] Sample((TRegion Region, float MinimumDistance, int PointsPerIteration) param)
    {
        if (typeof(TPoint) == typeof(Vector2))
            return (TPoint[])(object)SamplePlane(param.Region, param.MinimumDistance, param.PointsPerIteration);

        if (typeof(TPoint) == typeof(Vector3))
            return (TPoint[])(object)SampleVolume(param.Region, param.MinimumDistance, param.PointsPerIteration);

        throw new NotSupportedException(
            $"UniformPoissonSamplingStrategy only supports Vector2 and Vector3, not {typeof(TPoint)}.");
    }

    private Vector2[] SamplePlane(TRegion region, float minimumDistance, int pointsPerIteration)
    {
        var min = ToVector2(region.Min);
        var max = ToVector2(region.Max);
        var dimensions = max - min;
        var cellSize = minimumDistance / MathConstants.SquareRootTwo;

        var settings = new PlaneSettings
        {
            Min = min,
            Dimensions = dimensions,
            CellSize = cellSize,
            MinimumDistance = minimumDistance,
            GridWidth = (int)(dimensions.X / cellSize) + 1,
            GridHeight = (int)(dimensions.Y / cellSize) + 1
        };

        var state = new PlaneState
        {
            Grid = new Vector2?[settings.GridWidth, settings.GridHeight],
            ActivePoints = new Vector2[InitialCapacity],
            ActivePointsCount = 0,
            Points = new Vector2[InitialCapacity],
            PointsCount = 0
        };

        AddFirstPointPlane(region, ref settings, ref state);

        while (state.ActivePointsCount != 0)
        {
            var listIndex = _randomSource.Next(state.ActivePointsCount);
            var point = state.ActivePoints[listIndex];
            var found = false;

            for (var k = 0; k < pointsPerIteration; k++)
                found |= AddNextPointPlane(region, point, ref settings, ref state);

            if (!found)
                state.ActivePoints[listIndex] = state.ActivePoints[--state.ActivePointsCount];
        }

        return Trim(state.Points, state.PointsCount);
    }

    private Vector3[] SampleVolume(TRegion region, float minimumDistance, int pointsPerIteration)
    {
        var min = ToVector3(region.Min);
        var max = ToVector3(region.Max);
        var dimensions = max - min;
        var cellSize = minimumDistance / MathConstants.SquareRootThree;

        var settings = new VolumeSettings
        {
            Min = min,
            Dimensions = dimensions,
            CellSize = cellSize,
            MinimumDistance = minimumDistance,
            GridWidth = (int)(dimensions.X / cellSize) + 1,
            GridHeight = (int)(dimensions.Y / cellSize) + 1,
            GridDepth = (int)(dimensions.Z / cellSize) + 1
        };

        var state = new VolumeState
        {
            Grid = new Vector3?[settings.GridWidth, settings.GridHeight, settings.GridDepth],
            ActivePoints = new Vector3[InitialCapacity],
            ActivePointsCount = 0,
            Points = new Vector3[InitialCapacity],
            PointsCount = 0
        };

        AddFirstPointVolume(region, ref settings, ref state);

        while (state.ActivePointsCount != 0)
        {
            var listIndex = _randomSource.Next(state.ActivePointsCount);
            var point = state.ActivePoints[listIndex];
            var found = false;

            for (var k = 0; k < pointsPerIteration; k++)
                found |= AddNextPointVolume(region, point, ref settings, ref state);

            if (!found)
                state.ActivePoints[listIndex] = state.ActivePoints[--state.ActivePointsCount];
        }

        return Trim(state.Points, state.PointsCount);
    }

    private void AddFirstPointPlane(TRegion region, ref PlaneSettings settings, ref PlaneState state)
    {
        while (true)
        {
            var p = new Vector2(
                (float)(settings.Min.X + settings.Dimensions.X * _randomSource.NextDouble()),
                (float)(settings.Min.Y + settings.Dimensions.Y * _randomSource.NextDouble()));

            if (!ContainsPlane(region, p))
                continue;

            var index = DenormalizePlane(p, settings.Min, settings.CellSize);
            state.Grid[(int)index.X, (int)index.Y] = p;
            Append(ref state.ActivePoints, ref state.ActivePointsCount, p);
            Append(ref state.Points, ref state.PointsCount, p);
            return;
        }
    }

    private void AddFirstPointVolume(TRegion region, ref VolumeSettings settings, ref VolumeState state)
    {
        while (true)
        {
            var p = new Vector3(
                (float)(settings.Min.X + settings.Dimensions.X * _randomSource.NextDouble()),
                (float)(settings.Min.Y + settings.Dimensions.Y * _randomSource.NextDouble()),
                (float)(settings.Min.Z + settings.Dimensions.Z * _randomSource.NextDouble()));

            if (!ContainsVolume(region, p))
                continue;

            var index = DenormalizeVolume(p, settings.Min, settings.CellSize);
            state.Grid[(int)index.X, (int)index.Y, (int)index.Z] = p;
            Append(ref state.ActivePoints, ref state.ActivePointsCount, p);
            Append(ref state.Points, ref state.PointsCount, p);
            return;
        }
    }

    private bool AddNextPointPlane(TRegion region, Vector2 point, ref PlaneSettings settings, ref PlaneState state)
    {
        var q = GenerateRandomAroundPlane(point, settings.MinimumDistance);
        if (!ContainsPlane(region, q))
            return false;

        var qIndex = DenormalizePlane(q, settings.Min, settings.CellSize);

        for (var i = (int)Math.Max(0, qIndex.X - 2); i < Math.Min(settings.GridWidth, qIndex.X + 3); i++)
            for (var j = (int)Math.Max(0, qIndex.Y - 2); j < Math.Min(settings.GridHeight, qIndex.Y + 3); j++)
                if (state.Grid[i, j].HasValue && Vector2.Distance(state.Grid[i, j]!.Value, q) < settings.MinimumDistance)
                    return false;

        Append(ref state.ActivePoints, ref state.ActivePointsCount, q);
        Append(ref state.Points, ref state.PointsCount, q);
        state.Grid[(int)qIndex.X, (int)qIndex.Y] = q;
        return true;
    }

    private bool AddNextPointVolume(TRegion region, Vector3 point, ref VolumeSettings settings, ref VolumeState state)
    {
        var q = GenerateRandomAroundVolume(point, settings.MinimumDistance);
        if (!ContainsVolume(region, q))
            return false;

        var qIndex = DenormalizeVolume(q, settings.Min, settings.CellSize);

        for (var i = (int)Math.Max(0, qIndex.X - 2); i < Math.Min(settings.GridWidth, qIndex.X + 3); i++)
            for (var j = (int)Math.Max(0, qIndex.Y - 2); j < Math.Min(settings.GridHeight, qIndex.Y + 3); j++)
                for (var k = (int)Math.Max(0, qIndex.Z - 2); k < Math.Min(settings.GridDepth, qIndex.Z + 3); k++)
                    if (state.Grid[i, j, k].HasValue && Vector3.Distance(state.Grid[i, j, k]!.Value, q) < settings.MinimumDistance)
                        return false;

        Append(ref state.ActivePoints, ref state.ActivePointsCount, q);
        Append(ref state.Points, ref state.PointsCount, q);
        state.Grid[(int)qIndex.X, (int)qIndex.Y, (int)qIndex.Z] = q;
        return true;
    }

    private Vector2 GenerateRandomAroundPlane(Vector2 center, float minimumDistance)
    {
        var radius = minimumDistance + minimumDistance * _randomSource.NextDouble();
        var angle = MathConstants.TwoPi * _randomSource.NextDouble();
        return new Vector2(
            (float)(center.X + radius * Math.Sin(angle)),
            (float)(center.Y + radius * Math.Cos(angle)));
    }

    private Vector3 GenerateRandomAroundVolume(Vector3 center, float minimumDistance)
    {
        var radius = minimumDistance + minimumDistance * _randomSource.NextDouble();
        var theta = MathConstants.TwoPi * _randomSource.NextDouble();
        var phi = Math.Acos(2.0 * _randomSource.NextDouble() - 1.0);
        var sinPhi = Math.Sin(phi);

        return center + new Vector3(
            (float)(radius * sinPhi * Math.Cos(theta)),
            (float)(radius * sinPhi * Math.Sin(theta)),
            (float)(radius * Math.Cos(phi)));
    }

    private static bool ContainsPlane(TRegion region, Vector2 point) =>
        region.Contains(ToPoint(point));

    private static bool ContainsVolume(TRegion region, Vector3 point) =>
        region.Contains(ToPoint(point));

    private static TPoint ToPoint(Vector2 value) => (TPoint)(object)value;
    private static TPoint ToPoint(Vector3 value) => (TPoint)(object)value;
    private static Vector2 ToVector2(TPoint value) => (Vector2)(object)value!;
    private static Vector3 ToVector3(TPoint value) => (Vector3)(object)value!;

    private static Vector2 DenormalizePlane(Vector2 point, Vector2 origin, double cellSize) =>
        new((int)((point.X - origin.X) / cellSize), (int)((point.Y - origin.Y) / cellSize));

    private static Vector3 DenormalizeVolume(Vector3 point, Vector3 origin, double cellSize) =>
        new(
            (int)((point.X - origin.X) / cellSize),
            (int)((point.Y - origin.Y) / cellSize),
            (int)((point.Z - origin.Z) / cellSize));

    private static void Append<T>(ref T[] array, ref int count, T value)
    {
        if (count == array.Length)
            Array.Resize(ref array, array.Length * 2);
        array[count++] = value;
    }

    private static T[] Trim<T>(T[] array, int count)
    {
        if (count == array.Length)
            return array;

        var result = new T[count];
        Array.Copy(array, result, count);
        return result;
    }

    private struct PlaneSettings
    {
        public Vector2 Min;
        public Vector2 Dimensions;
        public float MinimumDistance;
        public float CellSize;
        public int GridWidth, GridHeight;
    }

    private struct PlaneState
    {
        public Vector2?[,] Grid;
        public Vector2[] ActivePoints;
        public int ActivePointsCount;
        public Vector2[] Points;
        public int PointsCount;
    }

    private struct VolumeSettings
    {
        public Vector3 Min;
        public Vector3 Dimensions;
        public float MinimumDistance;
        public float CellSize;
        public int GridWidth, GridHeight, GridDepth;
    }

    private struct VolumeState
    {
        public Vector3?[,,] Grid;
        public Vector3[] ActivePoints;
        public int ActivePointsCount;
        public Vector3[] Points;
        public int PointsCount;
    }
}
