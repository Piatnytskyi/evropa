namespace Evropa.World.Infrastructure.Math.Implementations;

using System;
using System.Collections.Generic;
using System.Numerics;
using Evropa.World.Infrastructure.Math.Abstractions;

// See Red Blob Games: "Alternatives to Voronoi diagrams" and
// "Delaunay+Voronoi on a sphere".

public class PseudoVoronoiDiagramCalculator : IVoronoiDiagramCalculator
{
    private readonly record struct Incident(int Triangle, int SpokeA, int SpokeB);

    public Vector2[][] Calculate((Vector2, Vector2, Vector2)[] triangles)
    {
        if (triangles.Length == 0)
            return Array.Empty<Vector2[]>();

        var centroids = new Vector2[triangles.Length];
        var indexOf = new Dictionary<Vector2, int>();
        var fans = new List<List<Incident>>();

        int SiteOf(Vector2 vertex)
        {
            if (!indexOf.TryGetValue(vertex, out var index))
            {
                index = fans.Count;
                indexOf[vertex] = index;
                fans.Add(new List<Incident>());
            }
            return index;
        }

        for (int t = 0; t < triangles.Length; t++)
        {
            var (a, b, c) = triangles[t];
            int ia = SiteOf(a);
            int ib = SiteOf(b);
            int ic = SiteOf(c);

            centroids[t] = (a + b + c) / 3f;

            fans[ia].Add(new Incident(t, ib, ic));
            fans[ib].Add(new Incident(t, ia, ic));
            fans[ic].Add(new Incident(t, ia, ib));
        }

        var regions = new List<Vector2[]>(fans.Count);
        foreach (var fan in fans)
        {
            if (TryBuildRegion(fan, centroids, out var polygon))
                regions.Add(polygon);
        }
        return regions.ToArray();
    }

    private static bool TryBuildRegion(List<Incident> fan, Vector2[] centroids, out Vector2[] polygon)
    {
        polygon = Array.Empty<Vector2>();

        if (fan.Count < 3)
            return false;

        var spokeToFan = new Dictionary<int, List<int>>(fan.Count * 2);
        for (int i = 0; i < fan.Count; i++)
        {
            AddSpoke(spokeToFan, fan[i].SpokeA, i);
            AddSpoke(spokeToFan, fan[i].SpokeB, i);
        }

        foreach (var sharing in spokeToFan.Values)
        {
            if (sharing.Count != 2)
                return false;
        }

        var ordered = new Vector2[fan.Count];
        var visited = new bool[fan.Count];
        int current = 0;
        int arrivingSpoke = -1;

        for (int i = 0; i < fan.Count; i++)
        {
            var incident = fan[current];
            ordered[i] = centroids[incident.Triangle];
            visited[current] = true;

            int nextSpoke = incident.SpokeA == arrivingSpoke ? incident.SpokeB : incident.SpokeA;
            var sharing = spokeToFan[nextSpoke];
            int next = sharing[0] == current ? sharing[1] : sharing[0];

            if (next == 0)
            {
                if (i != fan.Count - 1)
                    return false;
                break;
            }

            if (visited[next])
                return false;

            arrivingSpoke = nextSpoke;
            current = next;
        }

        polygon = ordered;
        return true;
    }

    private static void AddSpoke(Dictionary<int, List<int>> spokeToFan, int spoke, int fanIndex)
    {
        if (!spokeToFan.TryGetValue(spoke, out var sharing))
            spokeToFan[spoke] = sharing = new List<int>(2);
        sharing.Add(fanIndex);
    }
}
