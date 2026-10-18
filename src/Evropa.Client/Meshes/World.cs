namespace Evropa.Client.Meshes;

using System;
using Evropa.World.Application.Planets;
using Godot;
using Microsoft.Extensions.DependencyInjection;

public partial class World : MeshInstance3D
{
	private Random _random = new Random();
	private const int SpherePointCount = 5000;
	private const float SphereRadius = 5f;
	private const float Jitter = 0f;

	public override void _Ready()
	{
		var surfaceGenerator = ServiceProviderFactory.ServiceProvider.GetRequiredService<IPlanetSurfaceGenerator>();
		var faces = surfaceGenerator.Generate(SpherePointCount, Jitter);

		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		foreach (var face in faces)
		{
			var v1 = face[0];
			var v2 = face[1];
			var v3 = face[2];
			var v4 = face[3];

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
