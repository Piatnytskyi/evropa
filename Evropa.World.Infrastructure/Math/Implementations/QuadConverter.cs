namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Evropa.World.Core.Structs;
using Evropa.World.Infrastructure.Math.Abstractions;

public class QuadConverter : IQuadConverter
{
    public (Vector2, Vector2, Vector2, Vector2)[] ConvertToQuads((Vector2, Vector2, Vector2)[] triangles)
    {
        if (triangles.Length == 0)
            return Array.Empty<(Vector2, Vector2, Vector2, Vector2)>();

        var faces = new Vector2[triangles.Length][];
        for (int i = 0; i < triangles.Length; i++)
        {
            faces[i] = new[] { triangles[i].Item1, triangles[i].Item2, triangles[i].Item3 };
        }

        var (quadFaces, remainingTriangles) = RemoveEdges(faces);

        var result = new (Vector2, Vector2, Vector2, Vector2)[remainingTriangles.Length * 3 + quadFaces.Length * 4];
        int writeIndex = 0;
        SubdivideTriangles(remainingTriangles, result, ref writeIndex);
        SubdivideQuads(quadFaces, result, ref writeIndex);

        return result;
    }

    private static (Vector2[][] quads, Vector2[][] remainingTriangles) RemoveEdges(
        Vector2[][] faces)
    {
        var edgeCounts = new Dictionary<Edge, int>(faces.Length * 3);
        var edgeOrder = new Edge[faces.Length * 3];
        int edgeOrderCount = 0;

        for (int i = 0; i < faces.Length; i++)
        {
            foreach (var edge in GetEdges(faces[i]))
            {
                if (edgeCounts.TryGetValue(edge, out var count))
                {
                    edgeCounts[edge] = count + 1;
                }
                else
                {
                    edgeCounts[edge] = 1;
                    edgeOrder[edgeOrderCount++] = edge;
                }
            }
        }

        var edgeToFaces = new Dictionary<Edge, int[]>(edgeCounts.Count);
        var edgeFill = new Dictionary<Edge, int>(edgeCounts.Count);
        foreach (var kvp in edgeCounts)
        {
            edgeToFaces[kvp.Key] = new int[kvp.Value];
            edgeFill[kvp.Key] = 0;
        }

        for (int i = 0; i < faces.Length; i++)
        {
            foreach (var edge in GetEdges(faces[i]))
            {
                var arr = edgeToFaces[edge];
                arr[edgeFill[edge]++] = i;
            }
        }

        var available = new HashSet<Edge>(edgeToFaces.Keys);
        var merged = new HashSet<int>();
        var quadFaces = new Vector2[faces.Length / 2][];
        int quadFacesCount = 0;

        for (int k = 0; k < edgeOrderCount; k++)
        {
            var edge = edgeOrder[k];
            if (!available.Contains(edge))
                continue;

            var faceIndices = edgeToFaces[edge];
            if (faceIndices.Length != 2)
                continue;

            int f1 = faceIndices[0], f2 = faceIndices[1];

            if (merged.Contains(f1) || merged.Contains(f2))
                continue;

            if (faces[f1].Length != 3 || faces[f2].Length != 3)
                continue;

            quadFaces[quadFacesCount++] = MergeTriangles(faces[f1], faces[f2], edge);
            merged.Add(f1);
            merged.Add(f2);

            foreach (var e in GetEdges(faces[f1]))
                available.Remove(e);
            foreach (var e in GetEdges(faces[f2]))
                available.Remove(e);
        }

        if (quadFacesCount != quadFaces.Length)
            Array.Resize(ref quadFaces, quadFacesCount);

        var remainingTriangles = new Vector2[faces.Length - merged.Count][];
        int remainingIndex = 0;
        for (int i = 0; i < faces.Length; i++)
        {
            if (!merged.Contains(i))
                remainingTriangles[remainingIndex++] = faces[i];
        }

        return (quadFaces, remainingTriangles);
    }

    private static Vector2[] MergeTriangles(Vector2[] face1, Vector2[] face2, Edge sharedEdge)
    {
        var other1 = face1.First(v => !v.Equals(sharedEdge.V1) && !v.Equals(sharedEdge.V2));
        var other2 = face2.First(v => !v.Equals(sharedEdge.V1) && !v.Equals(sharedEdge.V2));

        var quad = new[] { sharedEdge.V1, other1, sharedEdge.V2, other2 };
        if (SignedArea(quad) < 0)
            quad = new[] { sharedEdge.V1, other2, sharedEdge.V2, other1 };

        return quad;
    }

    private static float SignedArea(Vector2[] polygon)
    {
        float sum = 0;
        for (int i = 0; i < polygon.Length; i++)
        {
            var j = (i + 1) % polygon.Length;
            sum += polygon[i].X * polygon[j].Y - polygon[j].X * polygon[i].Y;
        }
        return sum;
    }

    private static void SubdivideTriangles(
        Vector2[][] triangles,
        (Vector2, Vector2, Vector2, Vector2)[] result,
        ref int writeIndex)
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

            result[writeIndex++] = (a, mAB, center, mCA);
            result[writeIndex++] = (b, mBC, center, mAB);
            result[writeIndex++] = (c, mCA, center, mBC);
        }
    }

    private static void SubdivideQuads(
        Vector2[][] quads,
        (Vector2, Vector2, Vector2, Vector2)[] result,
        ref int writeIndex)
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

            result[writeIndex++] = (a, mAB, center, mDA);
            result[writeIndex++] = (b, mBC, center, mAB);
            result[writeIndex++] = (c, mCD, center, mBC);
            result[writeIndex++] = (d, mDA, center, mCD);
        }
    }

    private static IEnumerable<Edge> GetEdges(Vector2[] face)
    {
        for (int i = 0; i < face.Length; i++)
        {
            yield return new Edge(face[i], face[(i + 1) % face.Length]);
        }
    }
}
