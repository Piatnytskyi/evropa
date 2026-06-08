namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Collections.Generic;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Abstractions;

public class QuadConverter : IQuadConverter
{
    public (Vector2, Vector2, Vector2, Vector2)[] ConvertToQuads((Vector2, Vector2, Vector2)[] triangles)
    {
        if (triangles.Length == 0)
            return Array.Empty<(Vector2, Vector2, Vector2, Vector2)>();

        var faces = Array.ConvertAll(triangles, t => new[] { t.Item1, t.Item2, t.Item3 });
        var (quads, remainingTris) = MergeAdjacentTriangles(faces);

        var result = new (Vector2, Vector2, Vector2, Vector2)[remainingTris.Count * 3 + quads.Count * 4];
        int idx = 0;
        foreach (var tri in remainingTris)
            SubdivideTriangle(tri, result, ref idx);
        foreach (var quad in quads)
            SubdivideQuad(quad, result, ref idx);
        return result;
    }

    private static (List<Vector2[]> quads, List<Vector2[]> remainingTris) MergeAdjacentTriangles(Vector2[][] faces)
    {
        var edgeToFaces = new Dictionary<(Vector2, Vector2), List<int>>();
        for (int i = 0; i < faces.Length; i++)
        {
            foreach (var edge in GetEdges(faces[i]))
            {
                if (!edgeToFaces.TryGetValue(edge, out var list))
                    edgeToFaces[edge] = list = new List<int>(2);
                list.Add(i);
            }
        }

        var available = new HashSet<(Vector2, Vector2)>(edgeToFaces.Keys);
        var merged = new HashSet<int>();
        var quads = new List<Vector2[]>();

        foreach (var (edge, faceList) in edgeToFaces)
        {
            if (!available.Contains(edge) || faceList.Count != 2)
                continue;

            int f1 = faceList[0], f2 = faceList[1];
            if (merged.Contains(f1) || merged.Contains(f2))
                continue;

            quads.Add(MergeTriangles(faces[f1], faces[f2], edge));
            merged.Add(f1);
            merged.Add(f2);

            foreach (var e in GetEdges(faces[f1])) available.Remove(e);
            foreach (var e in GetEdges(faces[f2])) available.Remove(e);
        }

        var remainingTris = new List<Vector2[]>();
        for (int i = 0; i < faces.Length; i++)
            if (!merged.Contains(i))
                remainingTris.Add(faces[i]);

        return (quads, remainingTris);
    }

    private static Vector2[] MergeTriangles(Vector2[] face1, Vector2[] face2, (Vector2 V1, Vector2 V2) edge)
    {
        var other1 = FindOther(face1, edge.V1, edge.V2);
        var other2 = FindOther(face2, edge.V1, edge.V2);
        var quad = new[] { edge.V1, other1, edge.V2, other2 };
        if (SignedArea(quad) < 0)
            (quad[1], quad[3]) = (quad[3], quad[1]);
        return quad;
    }

    private static Vector2 FindOther(Vector2[] face, Vector2 a, Vector2 b)
    {
        foreach (var v in face)
            if (v != a && v != b)
                return v;
        throw new InvalidOperationException("No other vertex found.");
    }

    private static float SignedArea(Vector2[] polygon)
    {
        float sum = 0;
        for (int i = 0; i < polygon.Length; i++)
        {
            int j = (i + 1) % polygon.Length;
            sum += polygon[i].X * polygon[j].Y - polygon[j].X * polygon[i].Y;
        }
        return sum;
    }

    private static void SubdivideTriangle(Vector2[] tri, (Vector2, Vector2, Vector2, Vector2)[] result, ref int idx)
    {
        var (a, b, c) = (tri[0], tri[1], tri[2]);
        var mAB = (a + b) * 0.5f;
        var mBC = (b + c) * 0.5f;
        var mCA = (c + a) * 0.5f;
        var center = (a + b + c) / 3f;
        result[idx++] = (a, mAB, center, mCA);
        result[idx++] = (b, mBC, center, mAB);
        result[idx++] = (c, mCA, center, mBC);
    }

    private static void SubdivideQuad(Vector2[] quad, (Vector2, Vector2, Vector2, Vector2)[] result, ref int idx)
    {
        var (a, b, c, d) = (quad[0], quad[1], quad[2], quad[3]);
        var mAB = (a + b) * 0.5f;
        var mBC = (b + c) * 0.5f;
        var mCD = (c + d) * 0.5f;
        var mDA = (d + a) * 0.5f;
        var center = (a + b + c + d) * 0.25f;
        result[idx++] = (a, mAB, center, mDA);
        result[idx++] = (b, mBC, center, mAB);
        result[idx++] = (c, mCD, center, mBC);
        result[idx++] = (d, mDA, center, mCD);
    }

    private static IEnumerable<(Vector2, Vector2)> GetEdges(Vector2[] face)
    {
        for (int i = 0; i < face.Length; i++)
        {
            var a = face[i];
            var b = face[(i + 1) % face.Length];
            yield return a.X < b.X || (a.X == b.X && a.Y <= b.Y) ? (a, b) : (b, a);
        }
    }
}
