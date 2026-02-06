namespace Evropa.Nodes;

using Evropa.World.Infrastructure;
using Evropa.World.Infrastructure.Math.Abstractions;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using System;

public partial class Settlement : MeshInstance3D
{
	private Random _random = new Random();

	public override void _Ready()
	{
		var diskSampler = ServiceProviderFactory.ServiceProvider.GetRequiredService<IDiskSampler>();
		var points = diskSampler.SampleCircle(new System.Numerics.Vector2(0, 0), 50, 5);
		var triangulation = ServiceProviderFactory.ServiceProvider.GetRequiredService<ITriangulation>();
		(System.Numerics.Vector2, System.Numerics.Vector2, System.Numerics.Vector2)[] triangles = triangulation.Triangulate(points.ToArray());
		
		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		foreach (var triangle in triangles)
		{
			// Convert to Godot Vector3
			var vertex1 = new Vector3(triangle.Item1.X, 1, triangle.Item1.Y);
			var vertex2 = new Vector3(triangle.Item2.X, 1, triangle.Item2.Y);
			var vertex3 = new Vector3(triangle.Item3.X, 1, triangle.Item3.Y);

			// Get random color for this triangle
			var color = new Color(
				(float)_random.NextDouble(),
				(float)_random.NextDouble(),
				(float)_random.NextDouble()
			);

			// Add vertices with color and UV coordinates
			// Reverse winding order to make triangles face upward
			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(0, 0));
			surfaceTool.AddVertex(vertex1);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(0.5f, 1));
			surfaceTool.AddVertex(vertex3);

			surfaceTool.SetColor(color);
			surfaceTool.SetUV(new Vector2(1, 0));
			surfaceTool.AddVertex(vertex2);
		}

		// Generate normals for proper lighting
		surfaceTool.GenerateNormals();

		// Create the mesh
		var arrayMesh = new ArrayMesh();
		surfaceTool.Commit(arrayMesh);
		Mesh = arrayMesh;

		// Create a basic material with vertex colors enabled
		var material = new StandardMaterial3D();
		material.VertexColorUseAsAlbedo = true;
		material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
		arrayMesh.SurfaceSetMaterial(0, material);
	}
}
