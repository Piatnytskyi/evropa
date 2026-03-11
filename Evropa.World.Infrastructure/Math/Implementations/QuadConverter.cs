namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Evropa.World.Core.Structs;
using Evropa.World.Infrastructure.Math.Abstractions;

public class QuadConverter : IQuadConverter
{
    public List<(Vector2, Vector2, Vector2, Vector2)> ConvertToQuads(List<(Vector2, Vector2, Vector2)> triangles)
    {
        if (triangles.Count == 0)
            return new List<(Vector2, Vector2, Vector2, Vector2)>();

        var faces = triangles
            .Select(t => new List<Vector2> { t.Item1, t.Item2, t.Item3 })
            .ToList();

        var (quadFaces, remainingTriangles) = RemoveEdges(faces);

        var result = new List<(Vector2, Vector2, Vector2, Vector2)>();
        SubdivideTriangles(remainingTriangles, result);
        SubdivideQuads(quadFaces, result);

        return result;
    }

    private static (List<List<Vector2>> quads, List<List<Vector2>> remainingTriangles) RemoveEdges(
        List<List<Vector2>> faces)
    {
        var edgeToFaces = new Dictionary<Edge, List<int>>();

        for (int i = 0; i < faces.Count; i++)
        {
            foreach (var edge in GetEdges(faces[i]))
            {
                if (!edgeToFaces.TryGetValue(edge, out var faceList))
                {
                    faceList = new List<int>();
                    edgeToFaces[edge] = faceList;
                }

                faceList.Add(i);
            }
        }

        var available = new HashSet<Edge>(edgeToFaces.Keys);
        var merged = new HashSet<int>();
        var quadFaces = new List<List<Vector2>>();

        foreach (var edge in edgeToFaces.Keys.ToList())
        {
            if (!available.Contains(edge))
                continue;

            var faceIndices = edgeToFaces[edge];
            if (faceIndices.Count != 2)
                continue;

            int f1 = faceIndices[0], f2 = faceIndices[1];

            if (merged.Contains(f1) || merged.Contains(f2))
                continue;

            if (faces[f1].Count != 3 || faces[f2].Count != 3)
                continue;

            var quad = MergeTriangles(faces[f1], faces[f2], edge);
            quadFaces.Add(quad);
            merged.Add(f1);
            merged.Add(f2);

            // Remove all six edges of both original triangles from the available set,
            // preventing any edge of the resulting quad from being removed.
            foreach (var e in GetEdges(faces[f1]))
                available.Remove(e);
            foreach (var e in GetEdges(faces[f2]))
                available.Remove(e);
        }

        var remainingTriangles = new List<List<Vector2>>();
        for (int i = 0; i < faces.Count; i++)
        {
            if (!merged.Contains(i))
                remainingTriangles.Add(faces[i]);
        }

        return (quadFaces, remainingTriangles);
    }

    private static List<Vector2> MergeTriangles(List<Vector2> face1, List<Vector2> face2, Edge sharedEdge)
    {
        var other1 = face1.First(v => !v.Equals(sharedEdge.V1) && !v.Equals(sharedEdge.V2));
        var other2 = face2.First(v => !v.Equals(sharedEdge.V1) && !v.Equals(sharedEdge.V2));

        var quad = new List<Vector2> { sharedEdge.V1, other1, sharedEdge.V2, other2 };
        if (SignedArea(quad) < 0)
            quad = new List<Vector2> { sharedEdge.V1, other2, sharedEdge.V2, other1 };

        return quad;
    }

    private static float SignedArea(List<Vector2> polygon)
    {
        float sum = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            var j = (i + 1) % polygon.Count;
            sum += polygon[i].X * polygon[j].Y - polygon[j].X * polygon[i].Y;
        }
        return sum;
    }

    private static void SubdivideTriangles(
        List<List<Vector2>> triangles,
        List<(Vector2, Vector2, Vector2, Vector2)> result)
    {
        foreach (var tri in triangles)
        {
            var a = tri[0];
            var b = tri[1];
            var c = tri[2];

            var mAB = (a + b) * 0.5f;
            var mBC = (b + c) * 0.5f;
            var mCA = (c + a) * 0.5f;
            var center = (a + b + c) / 3f;

            result.Add((a, mAB, center, mCA));
            result.Add((b, mBC, center, mAB));
            result.Add((c, mCA, center, mBC));
        }
    }

    private static void SubdivideQuads(
        List<List<Vector2>> quads,
        List<(Vector2, Vector2, Vector2, Vector2)> result)
    {
        foreach (var quad in quads)
        {
            var a = quad[0];
            var b = quad[1];
            var c = quad[2];
            var d = quad[3];

            var mAB = (a + b) * 0.5f;
            var mBC = (b + c) * 0.5f;
            var mCD = (c + d) * 0.5f;
            var mDA = (d + a) * 0.5f;
            var center = (a + b + c + d) * 0.25f;

            result.Add((a, mAB, center, mDA));
            result.Add((b, mBC, center, mAB));
            result.Add((c, mCD, center, mBC));
            result.Add((d, mDA, center, mCD));
        }
    }

    private static IEnumerable<Edge> GetEdges(List<Vector2> face)
    {
        for (int i = 0; i < face.Count; i++)
        {
            yield return new Edge(face[i], face[(i + 1) % face.Count]);
        }
    }
}
