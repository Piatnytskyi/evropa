namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Collections.Generic;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Abstractions;

public class LaplacianSmoother : IFaceSmoother
{
    private const float LaplacianWeight = 0.5f;
    private const float ShapeWeight = 0.35f;

    public Vector2[][] Smooth(Vector2[][] faces, int iterations = 10)
    {
        if (faces.Length == 0)
            return faces;

        var (positions, faceIndices) = BuildTopology(faces);
        int vertexCount = positions.Length;

        var neighbors = BuildAdjacency(faceIndices, vertexCount, out var pinned);

        var current = positions;
        for (int iter = 0; iter < iterations; iter++)
            current = Relax(current, faceIndices, neighbors, pinned);

        return Rebuild(current, faceIndices);
    }

    private static (Vector2[] positions, int[][] faceIndices) BuildTopology(Vector2[][] faces)
    {
        var indexOf = new Dictionary<Vector2, int>();
        var positions = new List<Vector2>();
        var faceIndices = new int[faces.Length][];

        for (int f = 0; f < faces.Length; f++)
        {
            var face = faces[f];
            var indices = new int[face.Length];
            for (int v = 0; v < face.Length; v++)
            {
                if (!indexOf.TryGetValue(face[v], out var index))
                {
                    index = positions.Count;
                    indexOf[face[v]] = index;
                    positions.Add(face[v]);
                }
                indices[v] = index;
            }
            faceIndices[f] = indices;
        }

        return (positions.ToArray(), faceIndices);
    }

    private static HashSet<int>[] BuildAdjacency(int[][] faceIndices, int vertexCount, out bool[] pinned)
    {
        var neighbors = new HashSet<int>[vertexCount];
        for (int i = 0; i < vertexCount; i++)
            neighbors[i] = new HashSet<int>();

        var edgeFaceCount = new Dictionary<(int, int), int>();
        foreach (var face in faceIndices)
        {
            for (int v = 0; v < face.Length; v++)
            {
                int a = face[v];
                int b = face[(v + 1) % face.Length];
                neighbors[a].Add(b);
                neighbors[b].Add(a);

                var edge = a < b ? (a, b) : (b, a);
                edgeFaceCount[edge] = edgeFaceCount.TryGetValue(edge, out var count) ? count + 1 : 1;
            }
        }

        pinned = new bool[vertexCount];
        foreach (var (edge, count) in edgeFaceCount)
        {
            if (count == 1)
            {
                pinned[edge.Item1] = true;
                pinned[edge.Item2] = true;
            }
        }

        return neighbors;
    }

    private static Vector2[] Relax(
        Vector2[] current,
        int[][] faceIndices,
        HashSet<int>[] neighbors,
        bool[] pinned)
    {
        var shapeTargetSum = new Vector2[current.Length];
        var shapeTargetCount = new int[current.Length];
        foreach (var face in faceIndices)
            AccumulateShapeTargets(face, current, shapeTargetSum, shapeTargetCount);

        var next = new Vector2[current.Length];
        for (int i = 0; i < current.Length; i++)
        {
            if (pinned[i] || neighbors[i].Count == 0)
            {
                next[i] = current[i];
                continue;
            }

            var p = current[i];

            var laplacian = Vector2.Zero;
            foreach (var neighbor in neighbors[i])
                laplacian += current[neighbor];
            laplacian /= neighbors[i].Count;

            var displacement = LaplacianWeight * (laplacian - p);

            if (shapeTargetCount[i] > 0)
            {
                var shapeTarget = shapeTargetSum[i] / shapeTargetCount[i];
                displacement += ShapeWeight * (shapeTarget - p);
            }

            next[i] = p + displacement;
        }

        return next;
    }

    private static void AccumulateShapeTargets(
        int[] face,
        Vector2[] positions,
        Vector2[] targetSum,
        int[] targetCount)
    {
        int n = face.Length;
        if (n < 3)
            return;

        float area = MathF.Abs(SignedArea(positions, face));
        if (area <= 0f)
            return;

        var centroid = Vector2.Zero;
        for (int i = 0; i < n; i++)
            centroid += positions[face[i]];
        centroid /= n;

        float step = 2f * MathF.PI / n;
        float radius = MathF.Sqrt(2f * area / (n * MathF.Sin(step)));

        float winding = MathF.Sign(SignedArea(positions, face));
        if (winding == 0f)
            winding = 1f;

        float dot = 0f, cross = 0f;
        for (int i = 0; i < n; i++)
        {
            var a = positions[face[i]] - centroid;
            float angle = step * i * winding;
            var b = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
            dot += a.X * b.X + a.Y * b.Y;
            cross += a.Y * b.X - a.X * b.Y;
        }

        float phi = MathF.Atan2(cross, dot);
        float cosPhi = MathF.Cos(phi), sinPhi = MathF.Sin(phi);

        for (int i = 0; i < n; i++)
        {
            float angle = step * i * winding;
            var b = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
            var rotated = new Vector2(b.X * cosPhi - b.Y * sinPhi, b.X * sinPhi + b.Y * cosPhi);

            int vertex = face[i];
            targetSum[vertex] += centroid + rotated;
            targetCount[vertex]++;
        }
    }

    private static float SignedArea(Vector2[] positions, int[] face)
    {
        float sum = 0f;
        for (int i = 0; i < face.Length; i++)
        {
            var p = positions[face[i]];
            var q = positions[face[(i + 1) % face.Length]];
            sum += p.X * q.Y - q.X * p.Y;
        }
        return sum * 0.5f;
    }

    private static Vector2[][] Rebuild(Vector2[] positions, int[][] faceIndices)
    {
        var result = new Vector2[faceIndices.Length][];
        for (int f = 0; f < faceIndices.Length; f++)
        {
            var indices = faceIndices[f];
            var face = new Vector2[indices.Length];
            for (int v = 0; v < indices.Length; v++)
                face[v] = positions[indices[v]];
            result[f] = face;
        }
        return result;
    }
}
