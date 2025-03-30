namespace Evropa.Nodes;

using Evropa.Infrastructure;
using Evropa.Infrastructure.Math.Abstractions;
using Godot;
using Microsoft.Extensions.DependencyInjection;

public partial class Settlement : Node
{
    private readonly IPoissonDiskSamplingFacade _poissonDiskSamplingFacade;

    public Settlement()
	{
		_poissonDiskSamplingFacade = ServiceProviderFactory.ServiceProvider.GetRequiredService<IPoissonDiskSamplingFacade>();
	}

	public override void _Ready()
	{
		var points = _poissonDiskSamplingFacade.GeneratePoissonDiskSampling(100, 100, 10.0f, 30);
		foreach (var point in points)
		{
			var cube = new MeshInstance3D();
			cube.Mesh = new BoxMesh();
			cube.Transform.Translated(new Vector3(point.X, point.Y, 1));
			AddChild(cube);
		}
	}
}
