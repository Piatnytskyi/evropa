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
	private const float Jitter = 0.5f;

	public override void _Ready()
	{
		var sphereGenerator = ServiceProviderFactory.ServiceProvider.GetRequiredService<IFibonacciSphereGenerator>();
		var spherePoints = sphereGenerator.GenerateSpherePoints(SpherePointCount, Jitter);

		var projection = ServiceProviderFactory.ServiceProvider.GetRequiredService<IStereographicProjection>();
		var planePoints = projection.ProjectToPlane(spherePoints);

		var triangulation = ServiceProviderFactory.ServiceProvider.GetRequiredService<ITriangulation>();
		var triangles2D = triangulation.Triangulate(planePoints.ToArray());

		var sphereTriangles = new List<(System.Numerics.Vector3, System.Numerics.Vector3, System.Numerics.Vector3)>();
		
		var planeToSphere = new Dictionary<System.Numerics.Vector2, System.Numerics.Vector3>();
		for (int i = 0; i < planePoints.Count; i++)
		{
			if (!planeToSphere.ContainsKey(planePoints[i]))
			{
				planeToSphere[planePoints[i]] = spherePoints[i];
			}
		}

		foreach (var triangle in triangles2D)
		{
			var v1 = planeToSphere.TryGetValue(triangle.Item1, out var s1) ? s1 : projection.ProjectToSphere(triangle.Item1);
			var v2 = planeToSphere.TryGetValue(triangle.Item2, out var s2) ? s2 : projection.ProjectToSphere(triangle.Item2);
			var v3 = planeToSphere.TryGetValue(triangle.Item3, out var s3) ? s3 : projection.ProjectToSphere(triangle.Item3);
			
			sphereTriangles.Add((v1, v2, v3));
		}

		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		foreach (var triangle in sphereTriangles)
		{
			var vertex1 = new Vector3(triangle.Item1.X, triangle.Item1.Y, triangle.Item1.Z) * SphereRadius;
			var vertex2 = new Vector3(triangle.Item2.X, triangle.Item2.Y, triangle.Item2.Z) * SphereRadius;
			var vertex3 = new Vector3(triangle.Item3.X, triangle.Item3.Y, triangle.Item3.Z) * SphereRadius;

			var color = new Color(
				(float)_random.NextDouble(),
				(float)_random.NextDouble(),
				(float)_random.NextDouble()
			);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(0, 0));
			surfaceTool.AddVertex(vertex1);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(0.5f, 1));
			surfaceTool.AddVertex(vertex2);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(1, 0));
			surfaceTool.AddVertex(vertex3);
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
