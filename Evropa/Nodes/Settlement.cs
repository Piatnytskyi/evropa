namespace Evropa.Nodes;

using Evropa.World.Infrastructure;
using Evropa.World.Infrastructure.Math.Abstractions;
using Godot;
using Microsoft.Extensions.DependencyInjection;

public partial class Settlement : Node
{
	public override void _Ready()
	{
		var uniformPoissonDiskSampler = ServiceProviderFactory.ServiceProvider.GetRequiredService<IDiskSampler>();
		var points = uniformPoissonDiskSampler.SampleCircle(new System.Numerics.Vector2(0, 0), 50, 5);
		foreach (var point in points)
		{
			var cube = new MeshInstance3D();
			cube.Mesh = new BoxMesh();
			cube.Transform = new Transform3D(Basis.Identity, new Vector3(point.X, 1, point.Y));

			AddChild(cube);
		}
	}
}
