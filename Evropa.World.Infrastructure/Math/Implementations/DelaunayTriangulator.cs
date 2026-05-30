namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Linq;
using System.Numerics;
using Evropa.Core.Constants;
using Evropa.World.Infrastructure.Math.Abstractions;

// Adapted from Delaunator by Vladimir Agafonkin.
// Ported to C# by Patryk Grech.
// 
// MIT License

// Copyright (c) 2019 Patryk Grech

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

public class DelaunayTriangulator : ITriangulator
{
    private readonly int[] EDGE_STACK = new int[512];

    private int[] triangles = [];

    private int[] halfedges = [];

    private Vector2[] points = [];
    private int[] hull = [];

    private int hashSize;
    private int[] hullPrev = [];
    private int[] hullNext = [];
    private int[] hullTri = [];
    private int[] hullHash = [];

    private float cx;
    private float cy;

    private int trianglesLen;
    private float[] coords = [];
    private int hullStart;
    private int hullSize;

    public DelaunayTriangulator()
    {

    }

    public (Vector2, Vector2, Vector2)[] Triangulate(Vector2[] points)
    {
        if (points.Length < 3)
        {
            throw new ArgumentOutOfRangeException("Need at least 3 points");
        }

        this.points = points;
        coords = new float[this.points.Length * 2];

        for (var i = 0; i < this.points.Length; i++)
        {
            var p = this.points[i];
            coords[2 * i] = p.X;
            coords[2 * i + 1] = p.Y;
        }

        var n = points.Length;
        var maxTriangles = 2 * n - 5;

        triangles = new int[maxTriangles * 3];

        halfedges = new int[maxTriangles * 3];
        hashSize = (int)Math.Ceiling(Math.Sqrt(n));

        hullPrev = new int[n];
        hullNext = new int[n];
        hullTri = new int[n];
        hullHash = new int[hashSize];

        var ids = new int[n];

        var minX = float.PositiveInfinity;
        var minY = float.PositiveInfinity;
        var maxX = float.NegativeInfinity;
        var maxY = float.NegativeInfinity;

        for (var i = 0; i < n; i++)
        {
            var x = coords[2 * i];
            var y = coords[2 * i + 1];
            if (x < minX) minX = x;
            if (y < minY) minY = y;
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
            ids[i] = i;
        }

        var cx = (minX + maxX) / 2;
        var cy = (minY + maxY) / 2;

        var minDist = float.PositiveInfinity;
        var i0 = 0;
        var i1 = 0;
        var i2 = 0;

        for (int i = 0; i < n; i++)
        {
            var d = Dist(cx, cy, coords[2 * i], coords[2 * i + 1]);
            if (d < minDist)
            {
                i0 = i;
                minDist = d;
            }
        }
        var i0x = coords[2 * i0];
        var i0y = coords[2 * i0 + 1];

        minDist = float.PositiveInfinity;

        for (int i = 0; i < n; i++)
        {
            if (i == i0) continue;
            var d = Dist(i0x, i0y, coords[2 * i], coords[2 * i + 1]);
            if (d < minDist && d > 0)
            {
                i1 = i;
                minDist = d;
            }
        }

        var i1x = coords[2 * i1];
        var i1y = coords[2 * i1 + 1];

        var minRadius = float.PositiveInfinity;

        for (int i = 0; i < n; i++)
        {
            if (i == i0 || i == i1) continue;
            var r = Circumradius(i0x, i0y, i1x, i1y, coords[2 * i], coords[2 * i + 1]);
            if (r < minRadius)
            {
                i2 = i;
                minRadius = r;
            }
        }
        var i2x = coords[2 * i2];
        var i2y = coords[2 * i2 + 1];

        if (minRadius == float.PositiveInfinity)
        {
            throw new Exception("No Delaunay triangulation exists for this input.");
        }

        if (Orient(i0x, i0y, i1x, i1y, i2x, i2y))
        {
            var i = i1;
            var x = i1x;
            var y = i1y;
            i1 = i2;
            i1x = i2x;
            i1y = i2y;
            i2 = i;
            i2x = x;
            i2y = y;
        }

        var center = Circumcenter(i0x, i0y, i1x, i1y, i2x, i2y);
        this.cx = center.X;
        this.cy = center.Y;

        var dists = new float[n];
        for (var i = 0; i < n; i++)
        {
            dists[i] = Dist(coords[2 * i], coords[2 * i + 1], center.X, center.Y);
        }

        Quicksort(ids, dists, 0, n - 1);

        hullStart = i0;
        hullSize = 3;

        hullNext[i0] = hullPrev[i2] = i1;
        hullNext[i1] = hullPrev[i0] = i2;
        hullNext[i2] = hullPrev[i1] = i0;

        hullTri[i0] = 0;
        hullTri[i1] = 1;
        hullTri[i2] = 2;

        hullHash[HashKey(i0x, i0y)] = i0;
        hullHash[HashKey(i1x, i1y)] = i1;
        hullHash[HashKey(i2x, i2y)] = i2;

        trianglesLen = 0;
        AddTriangle(i0, i1, i2, -1, -1, -1);

        float xp = 0;
        float yp = 0;

        for (var k = 0; k < ids.Length; k++)
        {
            var i = ids[k];
            var x = coords[2 * i];
            var y = coords[2 * i + 1];

            if (k > 0 && Math.Abs(x - xp) <= MathConstants.Epsilon && Math.Abs(y - yp) <= MathConstants.Epsilon) continue;
            xp = x;
            yp = y;

            if (i == i0 || i == i1 || i == i2) continue;

            var start = 0;
            for (var j = 0; j < hashSize; j++)
            {
                var key = HashKey(x, y);
                start = hullHash[(key + j) % hashSize];
                if (start != -1 && start != hullNext[start]) break;
            }


            start = hullPrev[start];
            var e = start;
            var q = hullNext[e];

            while (!Orient(x, y, coords[2 * e], coords[2 * e + 1], coords[2 * q], coords[2 * q + 1]))
            {
                e = q;
                if (e == start)
                {
                    e = int.MaxValue;
                    break;
                }

                q = hullNext[e];
            }

            if (e == int.MaxValue) continue;

            var t = AddTriangle(e, i, hullNext[e], -1, -1, hullTri[e]);

            hullTri[i] = Legalize(t + 2);
            hullTri[e] = t;
            hullSize++;

            var next = hullNext[e];
            q = hullNext[next];

            while (Orient(x, y, coords[2 * next], coords[2 * next + 1], coords[2 * q], coords[2 * q + 1]))
            {
                t = AddTriangle(next, i, q, hullTri[i], -1, hullTri[next]);
                hullTri[i] = Legalize(t + 2);
                hullNext[next] = next;
                hullSize--;
                next = q;

                q = hullNext[next];
            }

            if (e == start)
            {
                q = hullPrev[e];

                while (Orient(x, y, coords[2 * q], coords[2 * q + 1], coords[2 * e], coords[2 * e + 1]))
                {
                    t = AddTriangle(q, i, e, -1, hullTri[e], hullTri[q]);
                    Legalize(t + 2);
                    hullTri[q] = t;
                    hullNext[e] = e;
                    hullSize--;
                    e = q;

                    q = hullPrev[e];
                }
            }

            hullStart = hullPrev[i] = e;
            hullNext[e] = hullPrev[next] = i;
            hullNext[i] = next;

            hullHash[HashKey(x, y)] = i;
            hullHash[HashKey(coords[2 * e], coords[2 * e + 1])] = e;
        }

        hull = new int[hullSize];
        var s = hullStart;
        for (var i = 0; i < hullSize; i++)
        {
            hull[i] = s;
            s = hullNext[s];
        }

        triangles = triangles.Take(trianglesLen).ToArray();
        halfedges = halfedges.Take(trianglesLen).ToArray();

        var triCount = triangles.Length / 3;
        var result = new (Vector2, Vector2, Vector2)[triCount];
        for (var t = 0; t < triCount; t++)
        {
            var pts = GetTrianglePoints(t);
            result[t] = (pts[0], pts[1], pts[2]);
        }
        return result;
    }

    private int Legalize(int a)
    {
        var i = 0;
        int ar;

        while (true)
        {
            var b = halfedges[a];

            int a0 = a - a % 3;
            ar = a0 + (a + 2) % 3;

            if (b == -1)
            {
                if (i == 0) break;
                a = EDGE_STACK[--i];
                continue;
            }

            var b0 = b - b % 3;
            var al = a0 + (a + 1) % 3;
            var bl = b0 + (b + 2) % 3;

            var p0 = triangles[ar];
            var pr = triangles[a];
            var pl = triangles[al];
            var p1 = triangles[bl];

            var illegal = InCircle(
                coords[2 * p0], coords[2 * p0 + 1],
                coords[2 * pr], coords[2 * pr + 1],
                coords[2 * pl], coords[2 * pl + 1],
                coords[2 * p1], coords[2 * p1 + 1]);

            if (illegal)
            {
                triangles[a] = p1;
                triangles[b] = p0;

                var hbl = halfedges[bl];

                if (hbl == -1)
                {
                    var e = hullStart;
                    do
                    {
                        if (hullTri[e] == bl)
                        {
                            hullTri[e] = a;
                            break;
                        }
                        e = hullPrev[e];
                    } while (e != hullStart);
                }
                Link(a, hbl);
                Link(b, halfedges[ar]);
                Link(ar, bl);

                var br = b0 + (b + 1) % 3;

                if (i < EDGE_STACK.Length)
                {
                    EDGE_STACK[i++] = br;
                }
            }
            else
            {
                if (i == 0) break;
                a = EDGE_STACK[--i];
            }
        }

        return ar;
    }

    private static bool InCircle(float ax, float ay, float bx, float by, float cx, float cy, float px, float py)
    {
        var dx = ax - px;
        var dy = ay - py;
        var ex = bx - px;
        var ey = by - py;
        var fx = cx - px;
        var fy = cy - py;

        var ap = dx * dx + dy * dy;
        var bp = ex * ex + ey * ey;
        var cp = fx * fx + fy * fy;

        return dx * (ey * cp - bp * fy) -
                dy * (ex * cp - bp * fx) +
                ap * (ex * fy - ey * fx) < 0;
    }
    private int AddTriangle(int i0, int i1, int i2, int a, int b, int c)
    {
        var t = trianglesLen;

        triangles[t] = i0;
        triangles[t + 1] = i1;
        triangles[t + 2] = i2;

        Link(t, a);
        Link(t + 1, b);
        Link(t + 2, c);

        trianglesLen += 3;
        return t;
    }
    private void Link(int a, int b)
    {
        halfedges[a] = b;
        if (b != -1) halfedges[b] = a;
    }
    private int HashKey(float x, float y) => (int)(Math.Floor(PseudoAngle(x - cx, y - cy) * hashSize) % hashSize);
    private static float PseudoAngle(float dx, float dy)
    {
        var p = dx / (Math.Abs(dx) + Math.Abs(dy));
        return (dy > 0 ? 3 - p : 1 + p) / 4;
    }
    private static void Quicksort(int[] ids, float[] dists, int left, int right)
    {
        if (right - left <= 20)
        {
            for (var i = left + 1; i <= right; i++)
            {
                var temp = ids[i];
                var tempDist = dists[temp];
                var j = i - 1;
                while (j >= left && dists[ids[j]] > tempDist) ids[j + 1] = ids[j--];
                ids[j + 1] = temp;
            }
        }
        else
        {
            var median = (left + right) >> 1;
            var i = left + 1;
            var j = right;
            Swap(ids, median, i);
            if (dists[ids[left]] > dists[ids[right]]) Swap(ids, left, right);
            if (dists[ids[i]] > dists[ids[right]]) Swap(ids, i, right);
            if (dists[ids[left]] > dists[ids[i]]) Swap(ids, left, i);

            var temp = ids[i];
            var tempDist = dists[temp];
            while (true)
            {
                do i++; while (dists[ids[i]] < tempDist);
                do j--; while (dists[ids[j]] > tempDist);
                if (j < i) break;
                Swap(ids, i, j);
            }
            ids[left + 1] = ids[j];
            ids[j] = temp;

            if (right - i + 1 >= j - left)
            {
                Quicksort(ids, dists, i, right);
                Quicksort(ids, dists, left, j - 1);
            }
            else
            {
                Quicksort(ids, dists, left, j - 1);
                Quicksort(ids, dists, i, right);
            }
        }
    }
    private static void Swap(int[] arr, int i, int j)
    {
        var tmp = arr[i];
        arr[i] = arr[j];
        arr[j] = tmp;
    }
    private static bool Orient(float px, float py, float qx, float qy, float rx, float ry) => (qy - py) * (rx - qx) - (qx - px) * (ry - qy) < 0;
    private static float Circumradius(float ax, float ay, float bx, float by, float cx, float cy)
    {
        var dx = bx - ax;
        var dy = by - ay;
        var ex = cx - ax;
        var ey = cy - ay;
        var bl = dx * dx + dy * dy;
        var cl = ex * ex + ey * ey;
        var d = 0.5f / (dx * ey - dy * ex);
        var x = (ey * bl - dy * cl) * d;
        var y = (dx * cl - ex * bl) * d;
        return x * x + y * y;
    }
    private static Vector2 Circumcenter(float ax, float ay, float bx, float by, float cx, float cy)
    {
        var dx = bx - ax;
        var dy = by - ay;
        var ex = cx - ax;
        var ey = cy - ay;
        var bl = dx * dx + dy * dy;
        var cl = ex * ex + ey * ey;
        var d = 0.5f / (dx * ey - dy * ex);
        var x = ax + (ey * bl - dy * cl) * d;
        var y = ay + (dx * cl - ex * bl) * d;

        return new Vector2(x, y);
    }
    private static float Dist(float ax, float ay, float bx, float by)
    {
        var dx = ax - bx;
        var dy = ay - by;
        return dx * dx + dy * dy;
    }

    private Vector2[] GetTrianglePoints(int t)
    {
        var points = new Vector2[3];
        var i = 0;
        foreach (var p in PointsOfTriangle(t))
        {
            points[i++] = this.points[p];
        }
        return points;
    }

    private int[] PointsOfTriangle(int t)
    {
        var points = new int[3];
        var i = 0;
        foreach (var edge in EdgesOfTriangle(t))
        {
            points[i++] = triangles[edge];
        }
        return points;
    }

    private static int[] EdgesOfTriangle(int t) => new int[] { 3 * t, 3 * t + 1, 3 * t + 2 };
}
