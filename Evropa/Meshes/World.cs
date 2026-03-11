namespace Evropa.Meshes;

using System;
using System.Collections.Generic;
using Evropa.World.Infrastructure;
using Evropa.World.Infrastructure.Math.Abstractions;
using Godot;
using Microsoft.Extensions.DependencyInjection;

public partial class World : MeshInstance3D
{
	private Random _random = new Random();
	private const int SpherePointCount = 5000;
	private const float SphereRadius = 5f;
	private const float Jitter = 0;

	public override void _Ready()
	{
		var sphereGenerator = ServiceProviderFactory.ServiceProvider.GetRequiredService<ISphereSampler>();
		var spherePoints = sphereGenerator.SampleSphere(SpherePointCount, Jitter);

		var projection = ServiceProviderFactory.ServiceProvider.GetRequiredService<IStereographicProjector>();
		var planePoints = projection.ProjectToPlane(spherePoints);

		var triangulation = ServiceProviderFactory.ServiceProvider.GetRequiredService<ITriangulator>();
		var triangles2D = triangulation.Triangulate(planePoints);

		var quadConverter = ServiceProviderFactory.ServiceProvider.GetRequiredService<IQuadConverter>();
		var quads2D = quadConverter.ConvertToQuads(triangles2D);

		var planeToSphere = new Dictionary<System.Numerics.Vector2, System.Numerics.Vector3>();
		for (int i = 0; i < planePoints.Count; i++)
		{
			if (!planeToSphere.ContainsKey(planePoints[i]))
			{
				planeToSphere[planePoints[i]] = spherePoints[i];
			}
		}

		System.Numerics.Vector3 ToSphere(System.Numerics.Vector2 p) =>
			planeToSphere.TryGetValue(p, out var s) ? s : projection.ProjectToSphere(p);

		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		foreach (var quad in quads2D)
		{
			var v1 = ToSphere(quad.Item1);
			var v2 = ToSphere(quad.Item2);
			var v3 = ToSphere(quad.Item3);
			var v4 = ToSphere(quad.Item4);

			var vertex1 = new Vector3(v1.X, v1.Y, v1.Z) * SphereRadius;
			var vertex2 = new Vector3(v2.X, v2.Y, v2.Z) * SphereRadius;
			var vertex3 = new Vector3(v3.X, v3.Y, v3.Z) * SphereRadius;
			var vertex4 = new Vector3(v4.X, v4.Y, v4.Z) * SphereRadius;

			var faceCenter = (vertex1 + vertex2 + vertex3 + vertex4) * 0.25f;
			var normal = (vertex2 - vertex1).Cross(vertex3 - vertex1);
			if (normal.Dot(faceCenter) > 0)
			{
				(vertex2, vertex4) = (vertex4, vertex2);
			}

			var color = new Color(
				(float)_random.NextDouble(),
				(float)_random.NextDouble(),
				(float)_random.NextDouble()
			);

			// Triangle 1: v1, v2, v3
			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(0, 0));
			surfaceTool.AddVertex(vertex1);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(1, 0));
			surfaceTool.AddVertex(vertex2);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(1, 1));
			surfaceTool.AddVertex(vertex3);

			// Triangle 2: v1, v3, v4
			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(0, 0));
			surfaceTool.AddVertex(vertex1);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(1, 1));
			surfaceTool.AddVertex(vertex3);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(0, 1));
			surfaceTool.AddVertex(vertex4);
		}

		surfaceTool.GenerateNormals();

		var arrayMesh = new ArrayMesh();
		surfaceTool.Commit(arrayMesh);
		Mesh = arrayMesh;

        var material = new StandardMaterial3D
        {
            VertexColorUseAsAlbedo = true,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
        };
        arrayMesh.SurfaceSetMaterial(0, material);
	}
}
